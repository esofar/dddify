namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines a deletion timestamp.
/// </summary>
public interface IHasDeletedAt
{
    /// <summary>
    /// Gets or sets the timestamp when the current instance was soft deleted.
    /// </summary>
    DateTimeOffset? DeletedAt { get; set; }
}
