using System.Security.Claims;

namespace Dddify.Identity;

public static class CurrentUserExtensions
{
    public static Claim? FindClaim(this ICurrentUser currentUser, string claimType)
    {
        return currentUser?.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    public static Claim[] FindClaims(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? Array.Empty<Claim>();
    }

    public static Claim[] GetAllClaims(this ICurrentUser currentUser)
    {
        return currentUser.Principal?.Claims.ToArray() ?? Array.Empty<Claim>();
    }

    public static bool IsInRole(this ICurrentUser currentUser, string roleName)
    {
        return currentUser.FindClaims(DefaultClaimTypes.Role).Any(c => c.Value == roleName);
    }

    public static string? FindClaimValue(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.FindClaim(claimType)?.Value;
    }

    public static T FindClaimValue<T>(this ICurrentUser currentUser, string claimType)
        where T : struct
    {
        var value = currentUser.FindClaimValue(claimType);
        return value == null ? default : value.To<T>();
    }
}