namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines a last modification timestamp.
/// </summary>
public interface IHasModifiedAt
{
    /// <summary>
    /// Gets or sets the timestamp when the current instance was last modified.
    /// </summary>
    DateTimeOffset? ModifiedAt { get; set; }
}
