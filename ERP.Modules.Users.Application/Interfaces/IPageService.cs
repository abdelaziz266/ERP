using ERP.Modules.Users.Application.DTOs;
using ERP.SharedKernel.DTOs;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IPageService
{
    Task<ApiResponseDto<PageDto>> GetPageByIdAsync(Guid id);
    Task<ApiResponseDto<List<PageDto>>> GetAllPagesAsync();
    Task<ApiResponseDto<object>> DeletePageAsync(Guid id, Guid currentUserId);
}
