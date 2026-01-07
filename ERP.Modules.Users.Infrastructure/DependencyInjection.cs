using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ERP.Modules.Users.Infrastructure.Data;
using ERP.Modules.Users.Infrastructure.UnitOfWork;
using ERP.Modules.Users.Application.Interfaces;

namespace ERP.Modules.Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<UsersDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
        services.AddScoped<DataSeeder>();

        return services;
    }
}
