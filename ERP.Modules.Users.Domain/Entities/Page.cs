using ERP.SharedKernel.Data;

namespace ERP.Modules.Users.Domain.Entities;

public class Page : BaseEntity
{
    public string NameAr { get; private set; } = null!;
    public string NameEn { get; private set; } = null!;
    public string Key { get; private set; } = null!;
    public Guid? ParentId { get; private set; }

    public Page? Parent { get; private set; }
    public ICollection<Page> SubPages { get; private set; } = [];
    public virtual ICollection<RolePage> RolePages { get; set; } = [];

    private Page() { }

    public Page(string nameAr, string nameEn , string key, Guid? parentId = null)
    {
        Id = Guid.NewGuid();
        SetNameAr(nameAr);
        SetNameEn(nameEn);
        SetKey(nameEn);
        ParentId = parentId;
    }

    public void SetNameAr(string nameAr)
    {
        if (string.IsNullOrWhiteSpace(nameAr))
            throw new ArgumentException("Arabic name is required");

        NameAr = nameAr.Trim();
    }

    public void SetNameEn(string nameEn)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("English name is required");

        NameEn = nameEn.Trim();
    }
    public void SetKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Key is required");

        Key = key.Trim();
    }
}
