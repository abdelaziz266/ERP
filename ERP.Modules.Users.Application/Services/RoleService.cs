using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.Modules.Users.Application.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<Role> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ILocalizationService _localization;

    public RoleService(
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        ILocalizationService localization)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _localization = localization;
    }

    public async Task<RoleDto?> GetRoleByIdAsync(string id)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto?> GetRoleByNameAsync(string name)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == name && !r.IsDeleted);
        return role == null ? null : MapToDto(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleManager.Roles
            .Where(r => !r.IsDeleted)
            .ToListAsync();
        return roles.Select(MapToDto).ToList();
    }

    public async Task<PaginatedResponseDto<RoleDto>> GetAllRolesWithPaginationAsync(PaginationQueryDto query)
    {
        var rolesQuery = _roleManager.Roles.Where(r => !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            rolesQuery = rolesQuery.Where(r => r.Name.Contains(query.SearchTerm));
        }

        var totalCount = await rolesQuery.CountAsync();

        var paginatedRoles = await rolesQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        var message = _localization.GetMessage("roles.retrieved");

        return PaginatedResponseDto<RoleDto>.Success(
            paginatedRoles.Select(MapToDto).ToList(),
            query.PageNumber,
            query.PageSize,
            totalCount,
            message
        );
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, Language userLanguage = Language.en)
    {
        var roleExists = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == dto.Name && !r.IsDeleted);
        if (roleExists != null)
        {
            var message = _localization.GetMessage("role.already_exists", userLanguage);
            throw new AppException(string.Format(message, dto.Name), 409);
        }

        var role = new Role(dto.Name);
        role.SetCreated(Guid.Empty);

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to create role: {errors}", 400);
        }

        return MapToDto(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(string id, UpdateRoleDto dto, Language userLanguage = Language.en)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        if (role == null)
        {
            var message = _localization.GetMessage("role.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        if (!string.IsNullOrEmpty(dto.Name) && role.Name != dto.Name)
        {
            var roleExists = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == dto.Name && !r.IsDeleted && r.Id.ToString() != id);
            if (roleExists != null)
            {
                var message = _localization.GetMessage("role.already_exists", userLanguage);
                throw new AppException(string.Format(message, dto.Name), 409);
            }

            role.Name = dto.Name;
            role.NormalizedName = dto.Name.ToUpperInvariant();
        }

        role.SetUpdated(Guid.Empty);

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to update role: {errors}", 400);
        }

        return MapToDto(role);
    }

    public async Task DeleteRoleAsync(string id, Language userLanguage = Language.en)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        if (role == null)
        {
            var message = _localization.GetMessage("role.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any(u => !u.IsDeleted))
        {
            var message = _localization.GetMessage("role.has_users", userLanguage);
            throw new AppException(message, 400);
        }

        role.SetDeleted(Guid.Empty);

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to delete role: {errors}", 400);
        }
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            throw new AppException("User not found", 404);
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        var roleDtos = new List<RoleDto>();

        foreach (var roleName in roleNames)
        {
            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted);
            if (role != null)
            {
                roleDtos.Add(MapToDto(role));
            }
        }

        return roleDtos;
    }

    public async Task AssignRoleToUserAsync(Guid userId, string roleName, Language userLanguage = Language.en)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            throw new AppException("User not found", 404);
        }

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted);
        if (role == null)
        {
            var message = _localization.GetMessage("role.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        var isInRole = await _userManager.IsInRoleAsync(user, roleName);
        if (isInRole)
        {
            throw new AppException($"User is already in role {roleName}", 409);
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to assign role: {errors}", 400);
        }
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, string roleName, Language userLanguage = Language.en)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || user.IsDeleted)
        {
            throw new AppException("User not found", 404);
        }

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName && !r.IsDeleted);
        if (role == null)
        {
            var message = _localization.GetMessage("role.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        var isInRole = await _userManager.IsInRoleAsync(user, roleName);
        if (!isInRole)
        {
            throw new AppException($"User is not in role {roleName}", 409);
        }

        var result = await _userManager.RemoveFromRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to remove role: {errors}", 400);
        }
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id.ToString(),
            Name = role.Name ?? string.Empty,
            NormalizedName = role.NormalizedName ?? string.Empty,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt
        };
    }
}
