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
    private readonly IUsersUnitOfWork _unitOfWork;

    public RoleService(
        RoleManager<Role> roleManager,
        UserManager<User> userManager,
        ILocalizationService localization,
        IUsersUnitOfWork unitOfWork)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _localization = localization;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponseDto<RoleDto>> GetRoleByIdAsync(string id)
    {
        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        
        if (role == null)
        {
            throw new AppException(_localization.GetMessage("role.notfound"), 404);
        }

        return ApiResponseDto<RoleDto>.Success(MapToDto(role));
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
            rolesQuery = rolesQuery.Where(r => r.Name!.Contains(query.SearchTerm));
        }

        var totalCount = await rolesQuery.CountAsync();

        var paginatedRoles = await rolesQuery
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return PaginatedResponseDto<RoleDto>.Success(
            paginatedRoles.Select(MapToDto).ToList(),
            query.PageNumber,
            query.PageSize,
            totalCount,
            _localization.GetMessage("roles.retrieved")
        );
    }

    public async Task<ApiResponseDto<object>> CreateRoleAsync(CreateRoleDto dto, Guid currentUserId)
    {
        var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        var userLanguage = currentUser?.Language ?? Language.en;

        var roleExists = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == dto.Name && !r.IsDeleted);
        if (roleExists != null)
        {
            throw new AppException(string.Format(_localization.GetMessage("role.already_exists", userLanguage), dto.Name), 409);
        }

        var role = new Role(dto.Name);
        role.SetCreated(currentUserId);

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to create role: {errors}", 400);
        }

        return ApiResponseDto<object>.Success(null, _localization.GetMessage("role.created", userLanguage));
    }

    public async Task<ApiResponseDto<object>> UpdateRoleAsync(string id, UpdateRoleDto dto, Guid currentUserId)
    {
        var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        var userLanguage = currentUser?.Language ?? Language.en;

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        if (role == null)
        {
            throw new AppException(_localization.GetMessage("role.notfound", userLanguage), 404);
        }

        if (!string.IsNullOrEmpty(dto.Name) && role.Name != dto.Name)
        {
            var roleExists = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == dto.Name && !r.IsDeleted && r.Id.ToString() != id);
            if (roleExists != null)
            {
                throw new AppException(string.Format(_localization.GetMessage("role.already_exists", userLanguage), dto.Name), 409);
            }

            role.Name = dto.Name;
            role.NormalizedName = dto.Name.ToUpperInvariant();
        }

        role.SetUpdated(currentUserId);

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to update role: {errors}", 400);
        }

        return ApiResponseDto<object>.Success(null, _localization.GetMessage("role.updated", userLanguage));
    }

    public async Task<ApiResponseDto<object>> DeleteRoleAsync(string id, Guid currentUserId)
    {
        var currentUser = await _unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        var userLanguage = currentUser?.Language ?? Language.en;

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        if (role == null)
        {
            throw new AppException(_localization.GetMessage("role.notfound", userLanguage), 404);
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any(u => !u.IsDeleted))
        {
            throw new AppException(_localization.GetMessage("role.has_users", userLanguage), 400);
        }

        role.SetDeleted(currentUserId);

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to delete role: {errors}", 400);
        }

        return ApiResponseDto<object>.Success(null, _localization.GetMessage("role.deleted", userLanguage));
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
