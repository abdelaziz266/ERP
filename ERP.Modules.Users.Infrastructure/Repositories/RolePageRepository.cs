using ERP.Modules.Users.Domain.Entities;
using ERP.Modules.Users.Domain.Repositories;
using ERP.Modules.Users.Infrastructure.Data;
using ERP.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Modules.Users.Infrastructure.Repositories;

public class RolePageRepository : BaseRepository<RolePage>, IRolePageRepository
{
    public RolePageRepository(UsersDbContext context) : base(context)
    {
    }

    public override async Task<RolePage?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .Include(rp => rp.Page)
            .FirstOrDefaultAsync(rp => rp.Id == id && !rp.IsDeleted);
    }

    public async Task<IEnumerable<RolePage>> GetByRoleIdAsync(Guid roleId)
    {
        return await DbSet
            .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
            .Include(rp => rp.Page)
            .ToListAsync();
    }

    public async Task<IEnumerable<Guid>> GetPageIdsByRoleIdAsync(Guid roleId)
    {
        return await DbSet
            .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
            .Select(rp => rp.PageId)
            .ToListAsync();
    }

    public async Task<RolePage?> GetByRoleAndPageAsync(Guid roleId, Guid pageId)
    {
        return await DbSet
            .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PageId == pageId && !rp.IsDeleted);
    }

    public async Task DeleteByRoleIdAsync(Guid roleId)
    {
        var rolePages = await DbSet
            .Where(rp => rp.RoleId == roleId && !rp.IsDeleted)
            .ToListAsync();

        foreach (var rolePage in rolePages)
        {
            rolePage.SetDeleted(Guid.Empty);
        }
    }

    public async Task AddRangeAsync(IEnumerable<RolePage> rolePages)
    {
        await DbSet.AddRangeAsync(rolePages);
    }

    public async Task DeleteRangeAsync(IEnumerable<RolePage> rolePages)
    {
        foreach (var rolePage in rolePages)
        {
            rolePage.SetDeleted(Guid.Empty);
        }
        await Task.CompletedTask;
    }

    public override async Task DeleteAsync(RolePage entity)
    {
        entity.SetDeleted(Guid.Empty);
        await UpdateAsync(entity);
    }
}
