using ERP.Modules.Users.Domain.Entities;
using ERP.Modules.Users.Domain.Repositories;
using ERP.Modules.Users.Infrastructure.Data;
using ERP.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Modules.Users.Infrastructure.Repositories;

public class PageRepository : BaseRepository<Page>, IPageRepository
{
    public PageRepository(UsersDbContext context) : base(context)
    {
    }

    public override async Task<Page?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(p => p.SubPages.Where(s => !s.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public async Task<Page?> GetByIdWithSubPagesAsync(Guid id)
    {
        return await DbSet
            .Include(p => p.SubPages.Where(s => !s.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
    }

    public override async Task<IEnumerable<Page>> GetAllAsync()
    {
        return await DbSet
            .Where(p => !p.IsDeleted)
            .Include(p => p.SubPages.Where(s => !s.IsDeleted))
            .ToListAsync();
    }

    public async Task<IEnumerable<Page>> GetAllParentPagesAsync()
    {
        return await DbSet
            .Where(p => !p.IsDeleted && p.ParentId == null)
            .Include(p => p.SubPages.Where(s => !s.IsDeleted))
            .ToListAsync();
    }

    public async Task<bool> NameArExistsAsync(string nameAr, Guid? excludeId = null)
    {
        return await DbSet.AnyAsync(p => 
            p.NameAr == nameAr && 
            !p.IsDeleted && 
            (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<bool> NameEnExistsAsync(string nameEn, Guid? excludeId = null)
    {
        return await DbSet.AnyAsync(p => 
            p.NameEn == nameEn && 
            !p.IsDeleted && 
            (!excludeId.HasValue || p.Id != excludeId.Value));
    }

    public async Task<bool> HasSubPagesAsync(Guid pageId)
    {
        return await DbSet.AnyAsync(p => p.ParentId == pageId && !p.IsDeleted);
    }

    public override async Task DeleteAsync(Page entity)
    {
        entity.SetDeleted(Guid.Empty);
        await UpdateAsync(entity);
    }
}
