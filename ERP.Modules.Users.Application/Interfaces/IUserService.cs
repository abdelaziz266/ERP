using ERP.Modules.Users.Application.DTOs;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Enums;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<PaginatedResponseDto<UserDto>> GetAllUsersWithPaginationAsync(PaginationQueryDto query);
    Task<UserDto> CreateUserAsync(CreateUserDto dto, string? profilePicturePath = null, Language userLanguage = Language.en);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, string? profilePicturePath = null, Language userLanguage = Language.en);
    Task<UserDto> UpdateUserLanguageAsync(Guid userId, UpdateUserLanguageDto dto);
    Task<UserDto> UpdateUserProfilePictureAsync(Guid userId, string profilePicturePath, Language userLanguage = Language.en);
    Task DeleteUserAsync(Guid id, Language userLanguage = Language.en);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
}
