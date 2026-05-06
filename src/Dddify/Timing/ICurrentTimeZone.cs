namespace Dddify.Timing;

/// <summary>
/// Represents the effective time zone for the current request or execution context.
/// </summary>
public interface ICurrentTimeZone
{
    /// <summary>
    /// Gets the effective time zone.
    /// </summary>
    TimeZoneInfo TimeZone { get; }
}