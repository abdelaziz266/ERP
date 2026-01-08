using ERP.Modules.Users.Domain.Entities;
using ERP.Modules.Users.Infrastructure.Data;
using ERP.SharedKernel.Enums;
using Microsoft.AspNetCore.Identity;

namespace ERP.Modules.Users.Infrastructure.Data;

public class DataSeeder
{
    private readonly UsersDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public DataSeeder(
        UsersDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        await SeedIdentityRolesAsync();
        await SeedSuperAdminUserAsync();
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
            var superAdminUser = new User(
                fullName: "Super Administrator",
                email: "superadmin@system.com",
                gender: Gender.Male,
                birthday: new DateOnly(1990, 1, 1)
            );

            superAdminUser.SetUsername("superadmin");
            superAdminUser.IsActive = true;

            var result = await _userManager.CreateAsync(superAdminUser, "SuperAdmin@123");

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
            }
        }
    }
}
