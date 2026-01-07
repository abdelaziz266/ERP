using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class UpdateRoleDto
{
    [StringLength(256, ErrorMessage = "Role name cannot exceed 256 characters")]
    public string Name { get; set; }
}
