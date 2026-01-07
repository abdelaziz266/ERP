using Microsoft.Extensions.DependencyInjection;
using ERP.Modules.Users.Application.Interfaces;
using ERP.Modules.Users.Application.Services;

namespace ERP.Modules.Users.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersApplication(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}
