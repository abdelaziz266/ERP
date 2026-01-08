using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.Abstractions.Data;

namespace ERP.Modules.Users.Domain.Repositories;

public interface IPageRepository : IRepository<Page>
{
    Task<Page?> GetByIdWithSubPagesAsync(Guid id);
    Task<IEnumerable<Page>> GetAllParentPagesAsync();
    Task<bool> NameArExistsAsync(string nameAr, Guid? excludeId = null);
    Task<bool> NameEnExistsAsync(string nameEn, Guid? excludeId = null);
    Task<bool> HasSubPagesAsync(Guid pageId);
}
