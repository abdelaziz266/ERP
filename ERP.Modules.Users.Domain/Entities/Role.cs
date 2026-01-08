using Microsoft.AspNetCore.Identity;

namespace ERP.Modules.Users.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    public virtual ICollection<RolePage> RolePages { get; set; } = [];

    public Role() : base()
    {
        Id = Guid.NewGuid();
    }

    public Role(string roleName) : base(roleName)
    {
        Id = Guid.NewGuid();
        NormalizedName = roleName.ToUpperInvariant();
    }

    public void SetCreated(Guid userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdated(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDeleted(Guid userId)
    {
        IsDeleted = true;
        DeletedBy = userId;
        DeletedAt = DateTime.UtcNow;
    }
}
