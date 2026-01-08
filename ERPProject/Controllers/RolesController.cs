using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.SharedKernel.DTOs;
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

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponseDto<RoleDto>>> GetAllRoles([FromQuery] PaginationQueryDto query)
    {
        return Ok(await _roleService.GetAllRolesWithPaginationAsync(query));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRoleById(string id)
    {
        return Ok(await _roleService.GetRoleByIdAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponseDto<object>>> CreateRole([FromBody] CreateRoleDto dto)
    {
        return Ok(await _roleService.CreateRoleAsync(dto, User.GetUserIdAsGuid()));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> UpdateRole(string id, [FromBody] UpdateRoleDto dto)
    {
        return Ok(await _roleService.UpdateRoleAsync(id, dto, User.GetUserIdAsGuid()));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponseDto<object>>> DeleteRole(string id)
    {
        return Ok(await _roleService.DeleteRoleAsync(id, User.GetUserIdAsGuid()));
    }
}
