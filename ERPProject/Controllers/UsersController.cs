using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.SharedKernel.DTOs;
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

    public UsersController(IUserService userService, ILocalizationService localization)
    {
        _userService = userService;
        _localization = localization;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<IEnumerable<UserDto>>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponseDto<IEnumerable<UserDto>>.Success(users));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUserById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponseDto<UserDto>.Error($"User with ID {id} not found", 404));
        }
        return Ok(ApiResponseDto<UserDto>.Success(user));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> CreateUser([FromBody] CreateUserDto dto)
    {
        var userLanguage = User.GetUserLanguage();
        var user = await _userService.CreateUserAsync(dto);
        var message = _localization.GetMessage("user.created", userLanguage);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, 
            ApiResponseDto<UserDto>.Success(user, message, 201));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateUser(Guid id, [FromBody] UpdateUserDto dto)
    {
        var userLanguage = User.GetUserLanguage();
        var user = await _userService.UpdateUserAsync(id, dto);
        var message = _localization.GetMessage("user.updated", userLanguage);
        return Ok(ApiResponseDto<UserDto>.Success(user, message));
    }

    [HttpPut("language")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateUserLanguage([FromBody] UpdateUserLanguageDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponseDto<UserDto>.Error("Invalid token", 401));
        }

        var userLanguage = User.GetUserLanguage();
        var user = await _userService.UpdateUserLanguageAsync(userId, dto);
        var message = _localization.GetMessage("user.updated", userLanguage);
        return Ok(ApiResponseDto<UserDto>.Success(user, message));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
