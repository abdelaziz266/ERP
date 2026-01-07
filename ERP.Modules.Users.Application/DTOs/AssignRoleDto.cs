using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class AssignRoleDto
{
    [Required(ErrorMessage = "Role name is required")]
    public required string RoleName { get; set; }
}
