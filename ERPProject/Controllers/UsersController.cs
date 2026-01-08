using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.SharedKernel.DTOs;
using ERPProject.Extensions;

namespace ERPProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<UserDto>>> GetAllUsers([FromQuery] PaginationQueryDto query)
    {
        return Ok(await _userService.GetAllUsersWithPaginationAsync(query));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUserById(Guid id)
    {
        return Ok(await _userService.GetUserByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<object>>> CreateUser([FromForm] CreateUserDto dto, IFormFile? profilePicture)
    {
        return Ok(await _userService.CreateUserAsync(dto, profilePicture, User.GetUserIdAsGuid()));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateUser(Guid id, [FromForm] UpdateUserDto dto, IFormFile? profilePicture)
    {
        return Ok(await _userService.UpdateUserAsync(id, dto, profilePicture, User.GetUserIdAsGuid()));
    }

    [HttpPut("language")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateUserLanguage([FromBody] UpdateUserLanguageDto dto)
    {
        return Ok(await _userService.UpdateUserLanguageAsync(User.GetUserIdAsGuid(), dto));
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteUser(Guid id)
    {
        return Ok(await _userService.DeleteUserAsync(id, User.GetUserIdAsGuid()));
    }
}
