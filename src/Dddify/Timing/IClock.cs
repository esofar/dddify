namespace Dddify.Timing;

/// <summary>
/// Represents a service that provides the current time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current time in the effective time zone for the current request or execution context.
    /// </summary>
    DateTimeOffset Now { get; }

    /// <summary>
    /// Gets the current coordinated universal time.
    /// </summary>
    DateTimeOffset UtcNow { get; }
}
