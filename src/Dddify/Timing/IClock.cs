namespace Dddify.Timing;

/// <summary>
/// Represents a service that provides standard date and time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets Now.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets kind.
    /// </summary>
    DateTimeKind Kind { get; }

    /// <summary>
    /// Is that provider supports multiple time zone.
    /// </summary>
    bool SupportsMultipleTimezone { get; }

    /// <summary>
    /// Normalizes given <see cref="DateTime"/>.
    /// </summary>
    /// <param name="dateTime">The <see cref="DateTime"/> to be normalized.</param>
    /// <returns>The normalized <see cref="DateTime"/>.</returns>
    DateTime Normalize(DateTime dateTime);
}