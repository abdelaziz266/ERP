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
            .Include(r => r.RolePages.Where(rp => !rp.IsDeleted))
                .ThenInclude(rp => rp.Page)
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        
        if (role == null)
        {
            throw new AppException(_localization.Get("role.notfound"), 404);
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
        var rolesQuery = _roleManager.Roles
            .Where(r => !r.IsDeleted);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            rolesQuery = rolesQuery.Where(r => r.Name!.Contains(query.SearchTerm));
        }

        var totalCount = await rolesQuery.CountAsync();

        var paginatedRoles = await rolesQuery
            .Include(r => r.RolePages.Where(rp => !rp.IsDeleted))
                .ThenInclude(rp => rp.Page)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return PaginatedResponseDto<RoleDto>.Success(
            paginatedRoles.Select(MapToDto).ToList(),
            query.PageNumber,
            query.PageSize,
            totalCount,
            _localization.Get("roles.retrieved")
        );
    }

    public async Task<ApiResponseDto<object>> CreateRoleAsync(CreateRoleDto dto, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);

        var roleExists = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Name == dto.Name && !r.IsDeleted);
        if (roleExists != null)
        {
            throw new AppException(string.Format(_localization.Get("role.already_exists"), dto.Name), 409);
        }

        var role = new Role(dto.Name);
        role.SetCreated(currentUserId);

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to create role: {errors}", 400);
        }

        return ApiResponseDto<object>.Success(null, _localization.Get("role.created"));
    }

    public async Task<ApiResponseDto<object>> UpdateRoleAsync(string id, UpdateRoleDto dto, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        if (role == null)
        {
            throw new AppException(_localization.Get("role.notfound"), 404);
        }

        // Update role name if provided
        if (!string.IsNullOrEmpty(dto.Name) && role.Name != dto.Name)
        {
            var roleExists = await _roleManager.Roles
                .FirstOrDefaultAsync(r => r.Name == dto.Name && !r.IsDeleted && r.Id.ToString() != id);
            if (roleExists != null)
            {
                throw new AppException(string.Format(_localization.Get("role.already_exists"), dto.Name), 409);
            }

            role.Name = dto.Name;
            role.NormalizedName = dto.Name.ToUpperInvariant();
        }

        // Update role pages if provided
        if (dto.PageIds != null)
        {
            await UpdateRolePagesAsync(role.Id, dto.PageIds, currentUserId, userLanguage);
        }

        role.SetUpdated(currentUserId);

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to update role: {errors}", 400);
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<object>.Success(null, _localization.Get("role.updated"));
    }

    public async Task<ApiResponseDto<object>> DeleteRoleAsync(string id, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);

        var role = await _roleManager.Roles
            .FirstOrDefaultAsync(r => r.Id.ToString() == id && !r.IsDeleted);
        if (role == null)
        {
            throw new AppException(_localization.Get("role.notfound"), 404);
        }

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Any(u => !u.IsDeleted))
        {
            throw new AppException(_localization.Get("role.has_users"), 400);
        }

        // Delete role pages
        await _unitOfWork.RolePageRepository.DeleteByRoleIdAsync(role.Id);

        role.SetDeleted(currentUserId);

        var result = await _roleManager.UpdateAsync(role);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new AppException($"Failed to delete role: {errors}", 400);
        }

        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<object>.Success(null, _localization.Get("role.deleted"));
    }

    #region Private Methods

    private async Task<Language> GetUserLanguageAsync(Guid userId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        return user?.Language ?? Language.en;
    }

    private async Task UpdateRolePagesAsync(Guid roleId, List<Guid> newPageIds, Guid currentUserId, Language userLanguage)
    {
        // Validate all page IDs exist
        foreach (var pageId in newPageIds)
        {
            var page = await _unitOfWork.PageRepository.GetByIdAsync(pageId);
            if (page == null)
            {
                throw new AppException(_localization.Get("page.notfound"), 404);
            }
        }

        // Get current page IDs for this role
        var currentPageIds = (await _unitOfWork.RolePageRepository.GetPageIdsByRoleIdAsync(roleId)).ToList();

        // Find pages to add (in newPageIds but not in currentPageIds)
        var pageIdsToAdd = newPageIds.Except(currentPageIds).ToList();

        // Find pages to remove (in currentPageIds but not in newPageIds)
        var pageIdsToRemove = currentPageIds.Except(newPageIds).ToList();

        // Add new role pages
        if (pageIdsToAdd.Count > 0)
        {
            var rolePagesToAdd = pageIdsToAdd.Select(pageId =>
            {
                var rolePage = new RolePage(roleId, pageId);
                rolePage.SetCreated(currentUserId);
                return rolePage;
            });

            await _unitOfWork.RolePageRepository.AddRangeAsync(rolePagesToAdd);
        }

        // Remove old role pages
        if (pageIdsToRemove.Count > 0)
        {
            foreach (var pageId in pageIdsToRemove)
            {
                var rolePage = await _unitOfWork.RolePageRepository.GetByRoleAndPageAsync(roleId, pageId);
                if (rolePage != null)
                {
                    rolePage.SetDeleted(currentUserId);
                    await _unitOfWork.RolePageRepository.UpdateAsync(rolePage);
                }
            }
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
            UpdatedAt = role.UpdatedAt,
            Pages = role.RolePages?
                .Where(rp => !rp.IsDeleted && rp.Page != null && !rp.Page.IsDeleted)
                .Select(rp => new RolePageDto
                {
                    Id = rp.Page.Id,
                    NameAr = rp.Page.NameAr,
                    NameEn = rp.Page.NameEn,
                    Key = rp.Page.Key
                }).ToList() ?? []
        };
    }

    #endregion
}
