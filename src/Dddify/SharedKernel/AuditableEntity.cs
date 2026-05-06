using Dddify.SharedKernel.Auditing;

namespace Dddify.SharedKernel;

/// <summary>
/// Represents an entity with built-in auditing and soft-delete information.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class AuditableEntity<TKey> : Entity<TKey>, IAuditable
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the identifier of the user or actor who created the entity.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    public DateTimeOffset? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or actor who last modified the entity.
    /// </summary>
    public string? ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    public DateTimeOffset? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is soft deleted.
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or actor who soft deleted the entity.
    /// </summary>
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the entity was soft deleted.
    /// </summary>
    public DateTimeOffset? DeletedAt { get; set; }
}
