using ERP.Modules.Users.Domain.Entities;

namespace ERP.Modules.Users.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    int GetTokenExpirationMinutes();
}
