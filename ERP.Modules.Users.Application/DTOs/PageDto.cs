namespace ERP.Modules.Users.Application.DTOs;

public class PageDto
{
    public Guid Id { get; set; }
    public string NameAr { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public string Key { get; set; } = null!;
    public Guid? ParentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<PageDto> SubPages { get; set; } = [];
}
