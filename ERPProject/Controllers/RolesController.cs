using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Localization;
using ERPProject.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERPProject.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILocalizationService _localization;

    public RolesController(IRoleService roleService, ILocalizationService localization)
    {
        _roleService = roleService;
        _localization = localization;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<RoleDto>>> GetAllRoles([FromQuery] PaginationQueryDto query)
    {
        var result = await _roleService.GetAllRolesWithPaginationAsync(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRoleById(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        if (role == null)
        {
            return NotFound(ApiResponseDto<RoleDto>.Error("Role not found", 404));
        }
        return Ok(ApiResponseDto<RoleDto>.Success(role));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<object>>> CreateRole([FromBody] CreateRoleDto dto)
    {
        var userLanguage = User.GetUserLanguage();
        await _roleService.CreateRoleAsync(dto, userLanguage);
        var message = _localization.GetMessage("role.created", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateRole(string id, [FromBody] UpdateRoleDto dto)
    {
        var userLanguage = User.GetUserLanguage();
        await _roleService.UpdateRoleAsync(id, dto, userLanguage);
        var message = _localization.GetMessage("role.updated", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var userLanguage = User.GetUserLanguage();
        await _roleService.DeleteRoleAsync(id, userLanguage);
        var message = _localization.GetMessage("role.deleted", userLanguage);
        return Ok(ApiResponseDto<object>.Success(null, message));
    }
}
