using ERP.Modules.Users.Application.DTOs;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.DTOs;
using ERP.SharedKernel.Enums;
using ERP.SharedKernel.Exceptions;
using ERP.SharedKernel.Localization;

namespace ERP.Modules.Users.Application.Services;

public class PageService : IPageService
{
    private readonly IUsersUnitOfWork _unitOfWork;
    private readonly ILocalizationService _localization;

    public PageService(
        IUsersUnitOfWork unitOfWork,
        ILocalizationService localization)
    {
        _unitOfWork = unitOfWork;
        _localization = localization;
    }

    public async Task<ApiResponseDto<PageDto>> GetPageByIdAsync(Guid id)
    {
        var page = await _unitOfWork.PageRepository.GetByIdWithSubPagesAsync(id);
        if (page == null)
        {
            throw new AppException(_localization.GetMessage("page.notfound"), 404);
        }

        return ApiResponseDto<PageDto>.Success(MapToDto(page));
    }

    public async Task<ApiResponseDto<List<PageDto>>> GetAllPagesAsync()
    {
        var pages = await _unitOfWork.PageRepository.GetAllParentPagesAsync();
        var pageDtos = pages.Select(MapToDto).ToList();

        return ApiResponseDto<List<PageDto>>.Success(pageDtos, _localization.GetMessage("pages.retrieved"));
    }

    public async Task<ApiResponseDto<object>> DeletePageAsync(Guid id, Guid currentUserId)
    {
        var userLanguage = await GetUserLanguageAsync(currentUserId);

        var page = await _unitOfWork.PageRepository.GetByIdAsync(id);
        if (page == null)
        {
            throw new AppException(_localization.GetMessage("page.notfound", userLanguage), 404);
        }

        if (await _unitOfWork.PageRepository.HasSubPagesAsync(id))
        {
            throw new AppException(_localization.GetMessage("page.has_subpages", userLanguage), 400);
        }

        page.SetDeleted(currentUserId);
        await _unitOfWork.PageRepository.UpdateAsync(page);
        await _unitOfWork.SaveChangesAsync();

        return ApiResponseDto<object>.Success(null, _localization.GetMessage("page.deleted", userLanguage));
    }

    private async Task<Language> GetUserLanguageAsync(Guid userId)
    {
        var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
        return user?.Language ?? Language.en;
    }

    private static PageDto MapToDto(Page page)
    {
        return new PageDto
        {
            Id = page.Id,
            NameAr = page.NameAr,
            NameEn = page.NameEn,
            Key = page.Key,
            ParentId = page.ParentId,
            CreatedAt = page.CreatedAt,
            UpdatedAt = page.UpdatedAt,
            SubPages = page.SubPages?.Where(s => !s.IsDeleted).Select(MapToDto).ToList() ?? []
        };
    }
}
