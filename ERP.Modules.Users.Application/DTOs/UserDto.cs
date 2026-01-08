using ERP.SharedKernel.Enums;

namespace ERP.Modules.Users.Application.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public Gender Gender { get; set; }
    public DateOnly? Birthday { get; set; }
    public string? ProfilePicture { get; set; }
    public bool IsActive { get; set; }
    public Language Language { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public List<string> Roles { get; set; } = [];
}
