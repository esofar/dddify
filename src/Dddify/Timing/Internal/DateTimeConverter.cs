namespace Dddify.Timing.Internal;

/// <summary>
/// Provides helper methods for converting date and time values between UTC and effective presentation time zones.
/// </summary>
public sealed class DateTimeConverter(ICurrentTimeZone currentTimeZone) : IDateTimeConverter
{
    /// <inheritdoc />
    public DateTimeOffset ToCurrentTimeZone(DateTimeOffset value)
        => TimeZoneInfo.ConvertTime(value, currentTimeZone.TimeZone);

    /// <inheritdoc />
    public DateTimeOffset ToUtc(DateTimeOffset value)
        => value.ToUniversalTime();

    /// <inheritdoc />
    public DateTimeOffset ToTimeZone(DateTimeOffset value, TimeZoneInfo timeZone)
    {
        ArgumentNullException.ThrowIfNull(timeZone);

        return TimeZoneInfo.ConvertTime(value, timeZone);
    }
}
