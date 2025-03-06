using System.Security.Claims;

namespace Dddify.Users;

/// <summary>
/// Represents a service that provides current user information.
/// </summary>
public interface ICurrentUser
{
    /// <summary>
    /// Gets the <see cref="ClaimsPrincipal"/> representing the current user.
    /// </summary>
    ClaimsPrincipal? Principal { get; }

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    Guid? Id { get; }

    /// <summary>
    /// Gets the name of the current user.
    /// </summary>
    string? Name { get; }

    /// <summary>
    /// Gets the roles of the current user.
    /// </summary>
    IEnumerable<string>? Roles { get; }
}
