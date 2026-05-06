namespace Dddify.SharedKernel;

/// <summary>
/// Defines a soft-delete marker for an entity or aggregate root.
/// </summary>
/// <remarks>
/// Soft deletion marks data as deleted without physically removing it from persistent storage,
/// which allows later recovery, filtering, and auditing.
/// </remarks>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets a value indicating whether the current instance has been soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }
}
