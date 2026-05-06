namespace Dddify.Timing;

/// <summary>
/// Defines methods for converting date and time values between UTC and effective presentation time zones.
/// </summary>
public interface IDateTimeConverter
{
    /// <summary>
    /// Converts the specified time to the effective time zone for the current request or execution context.
    /// </summary>
    /// <param name="value">The time value to convert.</param>
    /// <returns>The converted time in the effective time zone.</returns>
    DateTimeOffset ToCurrentTimeZone(DateTimeOffset value);

    /// <summary>
    /// Converts the specified time to coordinated universal time.
    /// </summary>
    /// <param name="value">The time value to convert.</param>
    /// <returns>The converted time in coordinated universal time.</returns>
    DateTimeOffset ToUtc(DateTimeOffset value);

    /// <summary>
    /// Converts the specified time to the target time zone.
    /// </summary>
    /// <param name="value">The time value to convert.</param>
    /// <param name="timeZone">The target time zone.</param>
    /// <returns>The converted time in the target time zone.</returns>
    DateTimeOffset ToTimeZone(DateTimeOffset value, TimeZoneInfo timeZone);
}
