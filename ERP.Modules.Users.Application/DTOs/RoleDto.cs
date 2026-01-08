namespace ERP.Modules.Users.Application.DTOs;

public class RoleDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<RolePageDto> Pages { get; set; } = [];
}

public class RolePageDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public string Key { get; set; } = null!;
}
