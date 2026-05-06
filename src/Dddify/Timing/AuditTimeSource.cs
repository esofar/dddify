namespace Dddify.Timing;

/// <summary>
/// Defines which clock value should be used for audit timestamps.
/// </summary>
public enum AuditTimeSource
{
    /// <summary>
    /// Uses <see cref="IClock.UtcNow"/> for audit timestamps.
    /// </summary>
    UtcNow = 0,

    /// <summary>
    /// Uses <see cref="IClock.Now"/> for audit timestamps.
    /// </summary>
    Now = 1
}
