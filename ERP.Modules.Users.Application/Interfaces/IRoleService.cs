using ERP.Modules.Users.Application.DTOs;
using ERP.SharedKernel.DTOs;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IRoleService
{
    Task<RoleDto?> GetRoleByIdAsync(string id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<PaginatedResponseDto<RoleDto>> GetAllRolesWithPaginationAsync(RoleQueryDto query);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto dto, string userLanguage = "en");
    Task<RoleDto> UpdateRoleAsync(string id, UpdateRoleDto dto, string userLanguage = "en");
    Task DeleteRoleAsync(string id, string userLanguage = "en");
    Task<IEnumerable<RoleDto>> GetUserRolesAsync(Guid userId);
    Task AssignRoleToUserAsync(Guid userId, string roleName, string userLanguage = "en");
    Task RemoveRoleFromUserAsync(Guid userId, string roleName, string userLanguage = "en");
}
