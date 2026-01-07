using System.Security.Claims;

namespace ERPProject.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    public static Guid GetUserIdAsGuid(this ClaimsPrincipal user)
    {
        var id = user.GetUserId();
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }

    public static string GetUserLanguage(this ClaimsPrincipal user)
    {
        return user?.FindFirst("Language")?.Value ?? "en";
    }
}
