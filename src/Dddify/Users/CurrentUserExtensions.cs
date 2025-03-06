using System.Security.Claims;

namespace Dddify.Users;

/// <summary>
/// Provides extension methods for the <see cref="ICurrentUser"/> interface to work with claims.
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>
    /// Gets all claims of the current user.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <returns>An array of all claims, or an empty array if no claims are found.</returns>
    public static Claim[] GetAllClaims(this ICurrentUser currentUser)
    {
        return currentUser.Principal?.Claims.ToArray() ?? [];
    }

    /// <summary>
    /// Finds the first claim that matches the specified claim type.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <param name="claimType">The type of the claim to find.</param>
    /// <returns>The first matching claim, or null if no claim is found.</returns>
    public static Claim? FindClaim(this ICurrentUser currentUser, string claimType)
    {
        return currentUser?.Principal?.Claims.FirstOrDefault(c => c.Type == claimType);
    }

    /// <summary>
    /// Finds all claims that match the specified claim type.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <param name="claimType">The type of the claims to find.</param>
    /// <returns>An array of matching claims, or an empty array if no claims are found.</returns>
    public static Claim[] FindClaims(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.Principal?.Claims.Where(c => c.Type == claimType).ToArray() ?? [];
    }

    /// <summary>
    /// Finds the value of the first claim that matches the specified claim type.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <param name="claimType">The type of the claim to find.</param>
    /// <returns>The value of the first matching claim, or null if no claim is found.</returns>
    public static string? FindClaimValue(this ICurrentUser currentUser, string claimType)
    {
        return currentUser.FindClaim(claimType)?.Value;
    }

    /// <summary>
    /// Finds the value of the first claim that matches the specified claim type and converts it to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to convert the claim value to.</typeparam>
    /// <param name="currentUser">The current user.</param>
    /// <param name="claimType">The type of the claim to find.</param>
    /// <returns>The value of the first matching claim converted to the specified type, or the default value of the type if no claim is found.</returns>
    public static T FindClaimValue<T>(this ICurrentUser currentUser, string claimType)
        where T : struct
    {
        var value = currentUser.FindClaimValue(claimType);
        return value == null ? default : value.To<T>();
    }
}
