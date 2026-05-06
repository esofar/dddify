namespace Dddify.SharedKernel.Auditing;

/// <summary>
/// Defines a creation timestamp.
/// </summary>
public interface IHasCreatedAt
{
    /// <summary>
    /// Gets or sets the timestamp when the current instance was created.
    /// </summary>
    DateTimeOffset? CreatedAt { get; set; }
}
