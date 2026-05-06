namespace Dddify.Users;

/// <summary>
/// Provides helper methods for working with current-user abstractions.
/// </summary>
public static class CurrentUserExtensions
{
    /// <summary>
    /// Gets the current user identifier as a <see cref="Guid"/>.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <returns>The parsed <see cref="Guid"/> value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the current user identifier is missing or is not a valid <see cref="Guid"/>.
    /// </exception>
    public static Guid GetIdAsGuid(this ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (Guid.TryParse(currentUser.Id, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("The current user identifier is missing or is not a valid GUID.");
    }

    /// <summary>
    /// Gets the current user identifier as a <see cref="long"/>.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <returns>The parsed <see cref="long"/> value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the current user identifier is missing or is not a valid <see cref="long"/>.
    /// </exception>
    public static long GetIdAsLong(this ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (long.TryParse(currentUser.Id, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("The current user identifier is missing or is not a valid Int64.");
    }

    /// <summary>
    /// Gets the current user identifier as an <see cref="int"/>.
    /// </summary>
    /// <param name="currentUser">The current user.</param>
    /// <returns>The parsed <see cref="int"/> value.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the current user identifier is missing or is not a valid <see cref="int"/>.
    /// </exception>
    public static int GetIdAsInt(this ICurrentUser currentUser)
    {
        ArgumentNullException.ThrowIfNull(currentUser);

        if (int.TryParse(currentUser.Id, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("The current user identifier is missing or is not a valid Int32.");
    }
}
