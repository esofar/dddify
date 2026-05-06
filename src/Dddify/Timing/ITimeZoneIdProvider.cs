namespace Dddify.Timing;

/// <summary>
/// Provides the candidate time zone identifier for the current request or execution context.
/// </summary>
public interface ITimeZoneIdProvider
{
    /// <summary>
    /// Gets the candidate time zone identifier, or <see langword="null"/> when no context-specific value exists.
    /// </summary>
    string? GetTimeZoneId();
}
