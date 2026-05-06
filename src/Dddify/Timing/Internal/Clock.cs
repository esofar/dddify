namespace Dddify.Timing.Internal;

/// <summary>
/// Provides the current time using UTC as the storage baseline and the effective time zone for presentation.
/// </summary>
public class Clock(ICurrentTimeZone currentTimeZone) : IClock
{
    public virtual DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    public virtual DateTimeOffset Now => TimeZoneInfo.ConvertTime(UtcNow, currentTimeZone.TimeZone);
}
