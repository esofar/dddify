namespace Dddify.Timing;

/// <summary>
/// Resolves time zone identifiers into <see cref="TimeZoneInfo"/> instances.
/// </summary>
public interface ITimeZoneResolver
{
    /// <summary>
    /// Resolves the specified time zone identifier into a concrete <see cref="TimeZoneInfo"/>.
    /// </summary>
    /// <param name="timeZoneId">The time zone identifier to resolve.</param>
    /// <returns>The resolved <see cref="TimeZoneInfo"/>.</returns>
    TimeZoneInfo Resolve(string? timeZoneId);
}
