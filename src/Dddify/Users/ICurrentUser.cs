using System.Security.Claims;

namespace Dddify.Users;

/// <summary>
/// Represents the current user in the active execution context.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the identifier of the current user.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the claims associated with the current user.
    /// </summary>
    IReadOnlyCollection<Claim> Claims { get; }

    /// <summary>
    /// Gets the first claim value for the specified claim type.
    /// </summary>
    /// <param name="claimType">The claim type.</param>
    /// <returns>The claim value when found; otherwise, <see langword="null"/>.</returns>
    string? FindClaim(string claimType);
}
