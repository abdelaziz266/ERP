using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Interfaces;
using ERP.SharedKernel.Localization;
using ERPProject.Extensions;
using System.Security.Claims;

namespace ERPProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILocalizationService _localization;
    private readonly IFileService _fileService;

    public UsersController(IUserService userService, ILocalizationService localization, IFileService fileService)
    {
        _userService = userService;
        _localization = localization;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<UserDto>>> GetAllUsers([FromQuery] PaginationQueryDto query)
    {
        var result = await _userService.GetAllUsersWithPaginationAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponseDto<UserDto>.Error("User not found", 404));
        }
        return Ok(ApiResponseDto<UserDto>.Success(user));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<object>>> CreateUser([FromForm] CreateUserDto dto, IFormFile? profilePicture)
    {
        var userLanguage = User.GetUserLanguage();
        
        string? profilePicturePath = null;
        if (profilePicture != null && profilePicture.Length > 0)
        {
            using var stream = profilePicture.OpenReadStream();
            profilePicturePath = await _fileService.UploadFileAsync(stream, profilePicture.FileName, "profile-pictures");
        }

        await _userService.CreateUserAsync(dto, profilePicturePath, userLanguage);
        var message = _localization.GetMessage("user.created", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateUser(Guid id, [FromForm] UpdateUserDto dto, IFormFile? profilePicture)
    {
        var userLanguage = User.GetUserLanguage();

        string? profilePicturePath = null;
        if (profilePicture != null && profilePicture.Length > 0)
        {
            using var stream = profilePicture.OpenReadStream();
            profilePicturePath = await _fileService.UploadFileAsync(stream, profilePicture.FileName, "profile-pictures");
        }

        await _userService.UpdateUserAsync(id, dto, profilePicturePath, userLanguage);
        var message = _localization.GetMessage("user.updated", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }

    [HttpPut("{id:guid}/profile-picture")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateProfilePicture(Guid id, IFormFile profilePicture)
    {
        var userLanguage = User.GetUserLanguage();

        if (profilePicture == null || profilePicture.Length == 0)
        {
            return BadRequest(ApiResponseDto<object>.Error("Profile picture is required", 400));
        }

        using var stream = profilePicture.OpenReadStream();
        var profilePicturePath = await _fileService.UploadFileAsync(stream, profilePicture.FileName, "profile-pictures");

        await _userService.UpdateUserProfilePictureAsync(id, profilePicturePath, userLanguage);
        var message = _localization.GetMessage("user.updated", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }

    [HttpPut("language")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateUserLanguage([FromBody] UpdateUserLanguageDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponseDto<object>.Error("Invalid token", 401));
        }

        var userLanguage = User.GetUserLanguage();
        await _userService.UpdateUserLanguageAsync(userId, dto);
        var message = _localization.GetMessage("user.updated", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var userLanguage = User.GetUserLanguage();
        await _userService.DeleteUserAsync(id, userLanguage);
        var message = _localization.GetMessage("user.deleted", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }
}
