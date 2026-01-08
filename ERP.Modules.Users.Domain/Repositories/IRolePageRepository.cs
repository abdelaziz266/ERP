using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.Abstractions.Data;

namespace ERP.Modules.Users.Domain.Repositories;

public interface IRolePageRepository : IRepository<RolePage>
{
    Task<IEnumerable<RolePage>> GetByRoleIdAsync(Guid roleId);
    Task<IEnumerable<Guid>> GetPageIdsByRoleIdAsync(Guid roleId);
    Task<RolePage?> GetByRoleAndPageAsync(Guid roleId, Guid pageId);
    Task DeleteByRoleIdAsync(Guid roleId);
    Task AddRangeAsync(IEnumerable<RolePage> rolePages);
    Task DeleteRangeAsync(IEnumerable<RolePage> rolePages);
}
