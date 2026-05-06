namespace Dddify.Timing.Internal;

/// <summary>
/// Default implementation that does not provide a request-specific time zone identifier.
/// </summary>
public sealed class DefaultTimeZoneIdProvider : ITimeZoneIdProvider
{
    /// <inheritdoc />
    public string? GetTimeZoneId() => null;
}
