using ERP.Modules.Users.Domain.Entities;
using ERP.Modules.Users.Domain.Repositories;
using ERP.Modules.Users.Infrastructure.Data;
using ERP.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;

namespace ERP.Modules.Users.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(UsersDbContext context) : base(context)
    {
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
    }

    public override async Task<IEnumerable<User>> GetAllAsync()
    {
        return await DbSet
            .Where(u => !u.IsDeleted)
            .ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.Email == email.ToLower() && !u.IsDeleted);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await DbSet
            .FirstOrDefaultAsync(u => u.UserName == username.ToLower() && !u.IsDeleted);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await DbSet
            .AnyAsync(u => u.Email == email.ToLower() && !u.IsDeleted);
    }

    public override async Task DeleteAsync(User entity)
    {
        entity.SetDeleted(Guid.Empty);
        await UpdateAsync(entity);
    }
}
