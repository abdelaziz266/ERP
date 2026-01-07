using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Localization;
using Microsoft.AspNetCore.Identity;

namespace ERP.Modules.Users.Application.Services;

public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly UserManager<User> _userManager;
    private readonly ILocalizationService _localization;

    public RoleService(
        RoleManager<IdentityRole<Guid>> roleManager,
        UserManager<User> userManager,
        ILocalizationService localization)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _localization = localization;
    }

    public async Task<RoleDto?> GetRoleByIdAsync(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        return role == null ? null : MapToDto(role);
    }

    public async Task<RoleDto?> GetRoleByNameAsync(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);
        return role == null ? null : MapToDto(role);
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = _roleManager.Roles.ToList();
        return roles.Select(MapToDto).ToList();
    }

    public async Task<PaginatedResponseDto<RoleDto>> GetAllRolesWithPaginationAsync(RoleQueryDto query)
    {
        var roles = _roleManager.Roles.ToList();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            roles = roles.Where(r => r.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var totalCount = roles.Count;

        var paginatedRoles = roles
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(MapToDto)
            .ToList();

        var message = _localization.GetMessage("roles.retrieved", query.Language);

        return PaginatedResponseDto<RoleDto>.Success(
            paginatedRoles,
            query.PageNumber,
            query.PageSize,
            totalCount,
            message
        );
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, string userLanguage = "en")
    {
        var roleExists = await _roleManager.FindByNameAsync(dto.Name);
        if (roleExists != null)
        {
            var message = _localization.GetMessage("role.already_exists", userLanguage);
            throw new AppException(string.Format(message, dto.Name), 409);
        }

        var role = new IdentityRole<Guid>
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            NormalizedName = dto.Name.ToUpper()
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to create role: {errors}", 400);
        }

        return MapToDto(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(string id, UpdateRoleDto dto, string userLanguage = "en")
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            var message = _localization.GetMessage("role.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        if (!string.IsNullOrEmpty(dto.Name) && role.Name != dto.Name)
        {
            var roleExists = await _roleManager.FindByNameAsync(dto.Name);
            if (roleExists != null)
            {
                var message = _localization.GetMessage("role.already_exists", userLanguage);
                throw new AppException(string.Format(message, dto.Name), 409);
            }

            role.Name = dto.Name;
            role.NormalizedName = dto.Name.ToUpper();
        }

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to update role: {errors}", 400);
        }

        return MapToDto(role);
    }

    public async Task DeleteRoleAsync(string id, string userLanguage = "en")
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            var message = _localization.GetMessage("role.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to delete role: {errors}", 400);
        }
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(Guid userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new AppException("User not found", 404);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var roleDtos = new List<RoleDto>();

        foreach (var roleName in roles)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                roleDtos.Add(MapToDto(role));
            }
        }

        return roleDtos;
    }

    public async Task AssignRoleToUserAsync(Guid userId, string roleName, string userLanguage = "en")
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new AppException("User not found", 404);
        }

        var roleExists = await _roleManager.FindByNameAsync(roleName);
        if (roleExists == null)
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

    public async Task RemoveRoleFromUserAsync(Guid userId, string roleName, string userLanguage = "en")
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            throw new AppException("User not found", 404);
        }

        var roleExists = await _roleManager.FindByNameAsync(roleName);
        if (roleExists == null)
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

    private static RoleDto MapToDto(IdentityRole<Guid> role)
    {
        return new RoleDto
        {
            Id = role.Id.ToString(),
            Name = role.Name ?? string.Empty,
            NormalizedName = role.NormalizedName ?? string.Empty
        };
    }
}
