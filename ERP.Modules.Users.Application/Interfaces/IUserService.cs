using ERP.Modules.Users.Application.DTOs;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto);
    Task<UserDto> UpdateUserLanguageAsync(Guid userId, UpdateUserLanguageDto dto);
    Task DeleteUserAsync(Guid id);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto);
}
