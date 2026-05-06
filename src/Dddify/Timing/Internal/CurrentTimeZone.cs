namespace Dddify.Timing.Internal;

/// <summary>
/// Represents the effective time zone for the current request or execution context.
/// </summary>
public sealed class CurrentTimeZone : ICurrentTimeZone
{
    public CurrentTimeZone(ITimeZoneIdProvider timeZoneIdProvider, ITimeZoneResolver timeZoneResolver)
    {
        ArgumentNullException.ThrowIfNull(timeZoneIdProvider);
        ArgumentNullException.ThrowIfNull(timeZoneResolver);

        var candidateTimeZoneId = timeZoneIdProvider.GetTimeZoneId();
        TimeZone = timeZoneResolver.Resolve(candidateTimeZoneId);
    }

    public TimeZoneInfo TimeZone { get; }
}
