using ERP.Modules.Users.Application.DTOs;
using ERP.SharedKernel.DTOs;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IRoleService
{
    Task<ApiResponseDto<RoleDto>> GetRoleByIdAsync(string id);
    Task<PaginatedResponseDto<RoleDto>> GetAllRolesWithPaginationAsync(PaginationQueryDto query);
    Task<ApiResponseDto<object>> CreateRoleAsync(CreateRoleDto dto, Guid currentUserId);
    Task<ApiResponseDto<object>> UpdateRoleAsync(string id, UpdateRoleDto dto, Guid currentUserId);
    Task<ApiResponseDto<object>> DeleteRoleAsync(string id, Guid currentUserId);
}
