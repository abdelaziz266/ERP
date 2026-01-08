using ERP.SharedKernel.Data;

namespace ERP.Modules.Users.Domain.Entities;

public class RolePage : BaseEntity
{
    public Guid RoleId { get; private set; }
    public Guid PageId { get; private set; }

    public Role Role { get; private set; } = null!;
    public Page Page { get; private set; } = null!;

    private RolePage() { }

    public RolePage(Guid roleId, Guid pageId)
    {
        Id = Guid.NewGuid();
        RoleId = roleId;
        PageId = pageId;
    }
}
