using System.Security.Claims;

namespace Dddify.Users;

/// <summary>
/// Resolves the current user for the active execution context.
/// </summary>
public interface ICurrentUserProvider
{
    /// <summary>
    /// Gets the current claims principal for the active execution context.
    /// </summary>
    ClaimsPrincipal GetCurrentUser();
}
