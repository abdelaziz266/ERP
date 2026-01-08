using ERP.Modules.Users.Application.DTOs;
using ERP.SharedKernel.DTOs;
using Microsoft.AspNetCore.Http;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IUserService
{
    Task<ApiResponseDto<UserDto>> GetUserByIdAsync(Guid id);
    Task<PaginatedResponseDto<UserDto>> GetAllUsersWithPaginationAsync(PaginationQueryDto query);
    Task<ApiResponseDto<object>> CreateUserAsync(CreateUserDto dto, IFormFile? profilePicture, Guid currentUserId);
    Task<ApiResponseDto<object>> UpdateUserAsync(Guid id, UpdateUserDto dto, IFormFile? profilePicture, Guid currentUserId);
    Task<ApiResponseDto<object>> UpdateUserLanguageAsync(Guid currentUserId, UpdateUserLanguageDto dto);
    Task<ApiResponseDto<object>> DeleteUserAsync(Guid id, Guid currentUserId);
    Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
}
