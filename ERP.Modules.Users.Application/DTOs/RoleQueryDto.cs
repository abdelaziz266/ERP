namespace ERP.Modules.Users.Application.DTOs;

public class RoleQueryDto
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string Language { get; set; } = "en";
}
