using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class CreateRoleDto
{
    [Required(ErrorMessage = "Role name is required")]
    [StringLength(256, ErrorMessage = "Role name cannot exceed 256 characters")]
    public required string Name { get; set; }
}
