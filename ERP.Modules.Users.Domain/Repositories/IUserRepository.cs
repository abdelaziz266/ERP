using ERP.Modules.Users.Domain.Entities;
using ERP.SharedKernel.Abstractions.Data;

namespace ERP.Modules.Users.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
