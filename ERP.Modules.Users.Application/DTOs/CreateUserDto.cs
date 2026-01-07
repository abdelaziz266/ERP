using ERP.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class CreateUserDto
{
    [Required(ErrorMessage = "FullName is required")]
    [StringLength(256, ErrorMessage = "FullName cannot exceed 256 characters")]
    public required string FullName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(256, ErrorMessage = "Username cannot exceed 256 characters")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [StringLength(256, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 256 characters")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Gender is required")]
    public required Gender Gender { get; set; }

    public DateTime? Birthday { get; set; }

    public Guid? RoleId { get; set; }
}
