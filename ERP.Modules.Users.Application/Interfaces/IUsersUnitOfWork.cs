using ERP.Modules.Users.Domain.Repositories;
using ERP.SharedKernel.Abstractions.Data;

namespace ERP.Modules.Users.Application.Interfaces;

public interface IUsersUnitOfWork : IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IPageRepository PageRepository { get; }
    IRolePageRepository RolePageRepository { get; }
}
