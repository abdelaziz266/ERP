namespace ERP.Modules.Users.Application.DTOs;

public class RoleDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string NormalizedName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
