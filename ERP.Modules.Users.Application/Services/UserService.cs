using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Localization;
using Microsoft.AspNetCore.Identity;

namespace ERP.Modules.Users.Application.Services;

public class UserService : IUserService
{
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILocalizationService _localization;

    public UserService(
        IUsersUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        UserManager<User> userManager,
        ITokenService tokenService,
        ILocalizationService localization)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
        _tokenService = tokenService;
        _localization = localization;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync();
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        if (await _unitOfWork.UserRepository.EmailExistsAsync(dto.Email))
        {
            throw new AppException($"Email {dto.Email} is already in use", 409);
        }

        var user = new User(dto.FullName, dto.Email, dto.Gender, dto.Birthday);
        user.SetUsername(dto.Username);
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new AppException($"User with ID {id} not found", 404);
        }

        if (!string.IsNullOrEmpty(dto.FullName))
        {
            var property = typeof(User).GetProperty(nameof(User.FullName));
            property?.SetValue(user, dto.FullName);
        }

        if (!string.IsNullOrEmpty(dto.Email))
        {
            if (user.Email != dto.Email && await _unitOfWork.UserRepository.EmailExistsAsync(dto.Email))
            {
                throw new AppException($"Email {dto.Email} is already in use", 409);
            }
            user.Email = dto.Email;
            user.UserName = dto.Email.ToLowerInvariant();
        }

        if (!string.IsNullOrEmpty(dto.Username))
        {
            user.SetUsername(dto.Username);
        }

        if (!string.IsNullOrEmpty(dto.Password))
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);
        }

        if (dto.Gender != default)
        {
            user.SetGender(dto.Gender);
        }

        if (dto.Birthday.HasValue)
        {
            user.SetBirthday(dto.Birthday.Value);
        }

        if (dto.IsActive.HasValue)
        {
            user.SetIsActive(dto.IsActive.Value);
        }

        user.SetUpdated(Guid.Empty);

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task<UserDto> UpdateUserLanguageAsync(Guid userId, UpdateUserLanguageDto dto)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new AppException($"User with ID {userId} not found", 404);
        }

        user.SetLanguage(dto.Language);
        user.SetUpdated(userId);

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return MapToDto(user);
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new AppException($"User with ID {id} not found", 404);
        }

        await _unitOfWork.UserRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !user.IsActive)
        {
            var message = _localization.GetMessage("auth.invalid_credentials", user?.Language ?? "ar");
            throw new AppException(message, 401);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            var message = _localization.GetMessage("auth.invalid_credentials", user.Language);
            throw new AppException(message, 401);
        }

        var token = _tokenService.GenerateAccessToken(user);
        var expirationMinutes = _tokenService.GetTokenExpirationMinutes();

        return new LoginResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = expirationMinutes * 60,
            User = MapToDto(user)
        };
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            Gender = user.Gender,
            Birthday = user.Birthday,
            IsActive = user.IsActive,
            Language = user.Language,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted
        };
    }
}
