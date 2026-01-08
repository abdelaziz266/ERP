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
public class PagesController : ControllerBase
{
    private readonly IPageService _pageService;

    public PagesController(IPageService pageService)
    {
        _pageService = pageService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<List<PageDto>>>> GetAllPages()
    {
        return Ok(await _pageService.GetAllPagesAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponseDto<PageDto>>> GetPageById(Guid id)
    {
        return Ok(await _pageService.GetPageByIdAsync(id));
    }

    //[HttpDelete("{id:guid}")]
    //public async Task<ActionResult<ApiResponseDto<object>>> DeletePage(Guid id)
    //{
    //    return Ok(await _pageService.DeletePageAsync(id, User.GetUserIdAsGuid()));
    //}
}
