using Dddify.Timing.Internal;

namespace Dddify.Timing;

/// <summary>
/// Configures Dddify timing behavior.
/// </summary>
public sealed class TimingOptions
{
    /// <summary>
    /// Gets or sets which clock value should be used when populating audit timestamps.
    /// </summary>
    public AuditTimeSource AuditTimeSource { get; set; } = AuditTimeSource.UtcNow;

    /// <summary>
    /// Gets or sets the concrete type registered for <see cref="ITimeZoneIdProvider"/>.
    /// </summary>
    internal Type TimeZoneIdProviderType { get; set; } = typeof(DefaultTimeZoneIdProvider);

    /// <summary>
    /// Registers the specified implementation type for <see cref="ITimeZoneIdProvider"/>.
    /// </summary>
    public TimingOptions UseTimeZoneIdProvider<TTimeZoneIdProvider>()
        where TTimeZoneIdProvider : class, ITimeZoneIdProvider
    {
        TimeZoneIdProviderType = typeof(TTimeZoneIdProvider);
        return this;
    }

}
