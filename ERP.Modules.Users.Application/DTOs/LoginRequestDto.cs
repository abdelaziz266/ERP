using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    [StringLength(256, ErrorMessage = "Username cannot exceed 256 characters")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }
}
