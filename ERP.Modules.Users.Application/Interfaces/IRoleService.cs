using ERP.Modules.Users.Application.DTOs;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Enums;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IRoleService
{
    Task<RoleDto?> GetRoleByIdAsync(string id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<PaginatedResponseDto<RoleDto>> GetAllRolesWithPaginationAsync(PaginationQueryDto query);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, Language userLanguage = Language.en);
    Task<RoleDto> UpdateRoleAsync(string id, UpdateRoleDto dto, Language userLanguage = Language.en);
    Task DeleteRoleAsync(string id, Language userLanguage = Language.en);
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(Guid userId);
    Task AssignRoleToUserAsync(Guid userId, string roleName, Language userLanguage = Language.en);
    Task RemoveRoleFromUserAsync(Guid userId, string roleName, Language userLanguage = Language.en);
}
