using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Interfaces;
using ERP.SharedKernel.Localization;
using Microsoft.AspNetCore.Http;
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
    private readonly IFileService _fileService;

    public UserService(
        IUsersUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ITokenService tokenService,
        ILocalizationService localization,
        IFileService fileService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _localization = localization;
        _fileService = fileService;
    }

    public async Task<ApiResponseDto<UserDto>> GetUserByIdAsync(Guid id)
    {
        var user = await GetUserOrThrowAsync(id);
        var roles = await _userManager.GetRolesAsync(user);
        return ApiResponseDto<UserDto>.Success(MapToDto(user, roles.ToList()));
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

        return PaginatedResponseDto<UserDto>.Success(
            userDtos,
            query.PageNumber,
            query.PageSize,
            totalCount,
            _localization.Get("users.retrieved")
        );
    }

    public async Task<ApiResponseDto<object>> CreateUserAsync(CreateUserDto dto, IFormFile? profilePicture, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);

        await ValidateEmailNotExistsAsync(dto.Email, userLanguage);
        await ValidateUsernameNotExistsAsync(dto.Username, userLanguage);

        var role = dto.RoleId.HasValue 
            ? await GetRoleOrThrowAsync(dto.RoleId.Value, userLanguage) 
            : null;

        var passwordHash = _passwordHasher.HashPassword(null!, dto.Password);
        var newUser = new User(dto.FullName, dto.Email, dto.Username, passwordHash, dto.Gender, dto.Birthday);

        if (profilePicture != null)
        {
            var profilePicturePath = await _fileService.UploadImageAsync(profilePicture, "profile-pictures");
            newUser.SetProfilePicture(profilePicturePath);
        }

        await _unitOfWork.UserRepository.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();

        if (role != null)
        {
            await _userManager.AddToRoleAsync(newUser, role.Name!);
        }

        return ApiResponseDto<object>.Success(null, _localization.Get("user.created"));
    }

    public async Task<ApiResponseDto<object>> UpdateUserAsync(Guid id, UpdateUserDto dto, IFormFile? profilePicture, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);
        var user = await GetUserOrThrowAsync(id, userLanguage);

        if (!string.IsNullOrEmpty(dto.FullName))
        {
            var property = typeof(User).GetProperty(nameof(User.FullName));
            property?.SetValue(user, dto.FullName);
        }

        if (!string.IsNullOrEmpty(dto.Email) && user.Email != dto.Email)
        {
            await ValidateEmailNotExistsAsync(dto.Email, userLanguage);
            user.Email = dto.Email;
        }

        if (!string.IsNullOrEmpty(dto.Username) && user.UserName != dto.Username)
        {
            await ValidateUsernameNotExistsAsync(dto.Username, userLanguage);
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

        if (profilePicture != null)
        {
            var profilePicturePath = await _fileService.UploadImageAsync(profilePicture, "profile-pictures");
            user.SetProfilePicture(profilePicturePath);
        }

        if (dto.IsActive.HasValue)
        {
            user.SetIsActive(dto.IsActive.Value);
        }

        if (dto.RoleId.HasValue)
        {
            var newRole = await GetRoleOrThrowAsync(dto.RoleId.Value, userLanguage);

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            await _userManager.AddToRoleAsync(user, newRole.Name!);
        }

        user.SetUpdated(currentUserId);

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<object>.Success(null, _localization.Get("user.updated"));
    }

    public async Task<ApiResponseDto<object>> UpdateUserLanguageAsync(Guid currentUserId, UpdateUserLanguageDto dto)
    {
        var user = await GetUserOrThrowAsync(currentUserId);

        user.SetLanguage(dto.Language);
        user.SetUpdated(currentUserId);

        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<object>.Success(null, _localization.Get("user.updated", dto.Language));
    }

    public async Task<ApiResponseDto<object>> DeleteUserAsync(Guid id, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);
        var user = await GetUserOrThrowAsync(id);

        user.SetDeleted(currentUserId);
        await _unitOfWork.UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<object>.Success(null, _localization.Get("user.deleted"));
    }

    public async Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _unitOfWork.UserRepository.GetByUsernameAsync(dto.Username);
        if (user == null || !user.IsActive)
        {
            var lang = user?.Language ?? Language.ar;
            throw new AppException(_localization.Get("auth.invalid_credentials"), 401);
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            throw new AppException(_localization.Get("auth.invalid_credentials"), 401);
        }

        var token = _tokenService.GenerateAccessToken(user);
        var expirationDays = _tokenService.GetTokenExpirationDays();

        var roles = await _userManager.GetRolesAsync(user);
        var response = new LoginResponseDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = expirationDays * 24 * 60 * 60,
            User = MapToDto(user, roles.ToList())
        };

        return ApiResponseDto<LoginResponseDto>.Success(response, _localization.Get("user.login_success", user.Language));
    }

    #region Private Methods

    private async Task<Language> GetUserLanguageAsync(Guid userId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        return user?.Language ?? Language.en;
    }

    private async Task<User> GetUserOrThrowAsync(Guid id, Language? language = null)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new AppException(_localization.Get("user.notfound", language ?? Language.en), 404);
        }
        return user;
    }

    private async Task<Role> GetRoleOrThrowAsync(Guid roleId, Language language)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null || role.IsDeleted)
        {
            throw new AppException(_localization.Get("role.notfound", language), 404);
        }
        return role;
    }

    private async Task ValidateEmailNotExistsAsync(string email, Language language)
    {
        if (await _unitOfWork.UserRepository.EmailExistsAsync(email))
        {
            throw new AppException(string.Format(_localization.Get("user.email_exists", language), email), 409);
        }
    }

    private async Task ValidateUsernameNotExistsAsync(string username, Language language)
    {
        if (await _unitOfWork.UserRepository.UserExistsAsync(username))
        {
            throw new AppException(string.Format(_localization.Get("user.userName_exists", language), username), 409);
        }
    }

    private UserDto MapToDto(User user, List<string>? roles = null)
    {
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Username = user.UserName ?? string.Empty,
            Gender = user.Gender,
            Birthday = user.Birthday,
            ProfilePicture = _fileService.GetFileUrl(user.ProfilePicture ?? string.Empty),
            IsActive = user.IsActive,
            Language = user.Language,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted,
            Roles = roles ?? []
        };
    }

    #endregion
}
