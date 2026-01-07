using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Localization;
using Microsoft.AspNetCore.Identity;

namespace ERP.Modules.Users.Application.Services;

public class UserService : IUserService
{
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly ILocalizationService _localization;

    public UserService(
        IUsersUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ITokenService tokenService,
        ILocalizationService localization)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _localization = localization;
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _unitOfWork.UserRepository.GetByEmailAsync(email);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync();
        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(MapToDto(user, roles.ToList()));
        }

        return userDtos;
    }

    public async Task<PaginatedResponseDto<UserDto>> GetAllUsersWithPaginationAsync(PaginationQueryDto query)
    {
        var users = await _unitOfWork.UserRepository.GetAllAsync();
        var usersList = users.ToList();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            usersList = usersList.Where(u =>
                u.FullName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                (u.Email?.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u.UserName?.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            ).ToList();
        }

        var totalCount = usersList.Count;

        var paginatedUsers = usersList
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var userDtos = new List<UserDto>();
        foreach (var user in paginatedUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userDtos.Add(MapToDto(user, roles.ToList()));
        }

        var message = _localization.GetMessage("users.retrieved");

        return PaginatedResponseDto<UserDto>.Success(
            userDtos,
            query.PageNumber,
            query.PageSize,
            totalCount,
            message
        );
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto, string? profilePicturePath = null, Language userLanguage = Language.en)
    {
        if (await _unitOfWork.UserRepository.EmailExistsAsync(dto.Email))
        {
            var message = _localization.GetMessage("user.email_exists", userLanguage);
            throw new AppException(string.Format(message, dto.Email), 409);
        }
        if (await _unitOfWork.UserRepository.UserExistsAsync(dto.Username))
        {
            var message = _localization.GetMessage("user.userName_exists", userLanguage);
            throw new AppException(string.Format(message, dto.Username), 409);
        }

        var user = new User(dto.FullName, dto.Email, dto.Gender, dto.Birthday);
        user.SetUsername(dto.Username);
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        if (!string.IsNullOrEmpty(profilePicturePath))
        {
            user.SetProfilePicture(profilePicturePath);
        }

        await _unitOfWork.UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        if (dto.RoleId.HasValue)
        {
            var role = await _roleManager.FindByIdAsync(dto.RoleId.Value.ToString());
            if (role != null && !role.IsDeleted)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
            }
        }

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto dto, string? profilePicturePath = null, Language userLanguage = Language.en)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            var message = _localization.GetMessage("user.notfound", userLanguage);
            throw new AppException(message, 404);
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
                var message = _localization.GetMessage("user.email_exists", userLanguage);
                throw new AppException(string.Format(message, dto.Email), 409);
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

        if (dto.Gender.HasValue)
        {
            user.SetGender(dto.Gender.Value);
        }

        if (dto.Birthday.HasValue)
        {
            user.SetBirthday(dto.Birthday.Value);
        }

        if (!string.IsNullOrEmpty(profilePicturePath))
        {
            user.SetProfilePicture(profilePicturePath);
        }

        if (dto.IsActive.HasValue)
        {
            user.SetIsActive(dto.IsActive.Value);
        }

        if (dto.RoleId.HasValue)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            var newRole = await _roleManager.FindByIdAsync(dto.RoleId.Value.ToString());
            if (newRole != null && !newRole.IsDeleted)
            {
                await _userManager.AddToRoleAsync(user, newRole.Name!);
            }
        }

        user.SetUpdated(Guid.Empty);

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
    }

    public async Task<UserDto> UpdateUserProfilePictureAsync(Guid userId, string profilePicturePath, Language userLanguage = Language.en)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user == null)
        {
            var message = _localization.GetMessage("user.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        user.SetProfilePicture(profilePicturePath);
        user.SetUpdated(userId);

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
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

        var roles = await _userManager.GetRolesAsync(user);
        return MapToDto(user, roles.ToList());
    }

    public async Task DeleteUserAsync(Guid id, Language userLanguage = Language.en)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            var message = _localization.GetMessage("user.notfound", userLanguage);
            throw new AppException(message, 404);
        }

        await _unitOfWork.UserRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto dto)
    {
        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !user.IsActive)
        {
            var lang = user?.Language ?? Language.ar;
            var message = _localization.GetMessage("auth.invalid_credentials", lang);
            throw new AppException(message, 401);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            var message = _localization.GetMessage("auth.invalid_credentials", user.Language);
            throw new AppException(message, 401);
        }

        var token = _tokenService.GenerateAccessToken(user);
        var expirationDays = _tokenService.GetTokenExpirationDays();

        var roles = await _userManager.GetRolesAsync(user);
        return new LoginResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = expirationDays * 24 * 60 * 60,
            User = MapToDto(user, roles.ToList())
        };
    }

    private static UserDto MapToDto(User user, List<string>? roles = null)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            Gender = user.Gender,
            Birthday = user.Birthday,
            ProfilePicture = user.ProfilePicture,
            IsActive = user.IsActive,
            Language = user.Language,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted,
            Roles = roles ?? []
        };
    }
}
