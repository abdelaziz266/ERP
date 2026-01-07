using Microsoft.AspNetCore.Identity;
using System;
using ERP.SharedKernel.Enums;

namespace ERP.Modules.Users.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FullName { get; private set; } = null!;
    public Gender Gender { get; private set; }
    public DateTime? Birthday { get; private set; }
    public bool IsActive { get; set; } = true;
    public Language Language { get; set; } = Language.en;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }

    private User() { }

    public User(string fullName, string email, Gender gender, DateTime? birthday = null)
    {
        Id = Guid.NewGuid();
        SetFullName(fullName);
        SetEmail(email);
        Gender = gender;
        Birthday = birthday;
    }

    private void SetFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("FullName is required");

        FullName = fullName.Trim();
    }

    private void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required");

        var normalizedEmail = email.Trim().ToLowerInvariant();
        Email = normalizedEmail;
        UserName = normalizedEmail;
    }

    public void SetCreated(Guid userId)
    {
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;
    }

    public void SetUpdated(Guid userId)
    {
        UpdatedBy = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDeleted(Guid userId)
    {
        IsDeleted = true;
        DeletedBy = userId;
        DeletedAt = DateTime.UtcNow;
    }

    public void SetGender(Gender gender)
    {
        Gender = gender;
    }

    public void SetBirthday(DateTime? birthday)
    {
        Birthday = birthday;
    }

    public void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void SetUsername(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required");

        UserName = username.Trim();
        NormalizedUserName = UserName.ToUpperInvariant();
    }

    public void SetLanguage(Language language)
    {
        Language = language;
    }
}
