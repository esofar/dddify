namespace Dddify.Timing.Internal;

/// <summary>
/// Resolves time zones with a final UTC fallback.
/// </summary>
public sealed class TimeZoneResolver : ITimeZoneResolver
{
    /// <inheritdoc />
    public TimeZoneInfo Resolve(string? timeZoneId)
    {
        if (TryResolve(timeZoneId, out var resolvedTimeZone))
        {
            return resolvedTimeZone;
        }

        return TimeZoneInfo.Utc;
    }

    private static bool TryResolve(string? timeZoneId, out TimeZoneInfo timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            timeZone = default!;
            return false;
        }

        if (string.Equals(timeZoneId, "UTC", StringComparison.OrdinalIgnoreCase))
        {
            timeZone = TimeZoneInfo.Utc;
            return true;
        }

        try
        {
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            return true;
        }
        catch (TimeZoneNotFoundException)
        {
        }
        catch (InvalidTimeZoneException)
        {
        }

        timeZone = default!;
        return false;
    }
}
