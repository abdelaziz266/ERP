using Microsoft.EntityFrameworkCore.Storage;
using ERP.Modules.Users.Infrastructure.Data;
using ERP.Modules.Users.Domain.Repositories;
using ERP.Modules.Users.Infrastructure.Repositories;
using ERP.Modules.Users.Application.Interfaces;

namespace ERP.Modules.Users.Infrastructure.UnitOfWork;

public class UsersUnitOfWork : IUsersUnitOfWork
{
    private readonly UsersDbContext _context;
    private IDbContextTransaction? _transaction;
    
    private IUserRepository? _userRepository;
    private IPageRepository? _pageRepository;

    public UsersUnitOfWork(UsersDbContext context)
    {
        _context = context;
    }

    public IUserRepository UserRepository
    {
        get
        {
            _userRepository ??= new UserRepository(_context);
            return _userRepository;
        }
    }

    public IPageRepository PageRepository
    {
        get
        {
            _pageRepository ??= new PageRepository(_context);
            return _pageRepository;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction != null;
    }

    public async Task<bool> CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
            return true;
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task<bool> RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
            return true;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null)
        {
            await _transaction.DisposeAsync();
        }
        await _context.DisposeAsync();
    }
}
