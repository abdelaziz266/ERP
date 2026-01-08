using ERP.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;

namespace ERP.Modules.Users.Application.DTOs;

public class UpdateUserDto
{
    [StringLength(256, ErrorMessage = "FullName cannot exceed 256 characters")]
    public string? FullName { get; set; }

    [EmailAddress(ErrorMessage = "Email format is invalid")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    public string? Email { get; set; }

    [StringLength(256, ErrorMessage = "Username cannot exceed 256 characters")]
    public string? Username { get; set; }

    [StringLength(256, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 256 characters")]
    public string? Password { get; set; }

    public Gender? Gender { get; set; }

    public DateOnly? Birthday { get; set; }

    public bool? IsActive { get; set; }

    public Guid? RoleId { get; set; }
}
