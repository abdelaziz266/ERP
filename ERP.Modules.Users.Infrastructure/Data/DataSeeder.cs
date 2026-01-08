using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.Modules.Users.Infrastructure.Data;

public class DataSeeder
{
    private readonly UsersDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IPasswordHasher<User> _passwordHasher;

    public DataSeeder(
        UsersDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync()
    {
        await SeedIdentityRolesAsync();
        await SeedSuperAdminUserAsync();
        await SeedPagesAsync();
    }

    private async Task SeedIdentityRolesAsync()
    {
        var superAdminIdentityRole = await _roleManager.FindByNameAsync("SuperAdmin");
        if (superAdminIdentityRole == null)
        {
            var role = new Role("SuperAdmin");
            role.SetCreated(Guid.Empty);
            await _roleManager.CreateAsync(role);
        }
    }

    private async Task SeedSuperAdminUserAsync()
    {
        if (!_context.Users.Any(u => u.Email == "superadmin@system.com" && !u.IsDeleted))
        {
            var passwordHash = _passwordHasher.HashPassword(null!, "SuperAdmin@123");
            var superAdminUser = new User(
                fullName: "Super Administrator",
                email: "superadmin@system.com",
                username: "superadmin",
                passwordHash: passwordHash,
                gender: Gender.Male,
                birthday: new DateOnly(1990, 1, 1)
            );

            superAdminUser.IsActive = true;

            var result = await _userManager.CreateAsync(superAdminUser);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
            }
        }
    }

    private async Task SeedPagesAsync()
    {
        if (await _context.Pages.AnyAsync(p => !p.IsDeleted))
            return;

        var pages = new List<(string NameAr, string NameEn, List<(string NameAr, string NameEn)> SubPages)>
        {
            ("«·’·«ÕÌ« ", "Roles", [
                ("≈÷«›…", "Add"),
                (" ⁄œÌ·", "Edit"),
                ("Õ–›", "Delete"),
            ]),
            ("«·„” Œœ„Ì‰", "Users", [
                ("≈÷«›…", "Add"),
                (" ⁄œÌ·", "Edit"),
                ("Õ–›", "Delete"),
            ]),
            ("«·’›Õ« ", "Pages", [
                ("⁄—÷", "View"),
            ])
        };

        foreach (var (nameAr, nameEn, subPages) in pages)
        {
            var parentPage = new Page(nameAr, nameEn, nameEn);
            parentPage.SetCreated(Guid.Empty);
            await _context.Pages.AddAsync(parentPage);
            await _context.SaveChangesAsync();

            foreach (var (subNameAr, subNameEn) in subPages)
            {
                var subPage = new Page($"{subNameAr} {nameAr}" , $"{subNameEn} {nameEn}" , $"{nameEn}_{subNameEn}", parentPage.Id);
                subPage.SetCreated(Guid.Empty);
                await _context.Pages.AddAsync(subPage);
            }
        }

        await _context.SaveChangesAsync();
    }
}
