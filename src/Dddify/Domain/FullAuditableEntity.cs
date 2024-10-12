namespace Dddify.Domain;

/// <summary>
/// Represents a fully auditable entity that tracks creation, modification, and soft-deletion information. 
/// </summary>
/// <remarks>
/// This class inherits from <see cref="Entity{TKey}"/> and implements <see cref="IFullAuditable"/> to provide complete audit capabilities.
/// </remarks>
/// <typeparam name="TKey">The type of the primary key for the entity.</typeparam>
public abstract class FullAuditableEntity<TKey> : Entity<TKey>, IFullAuditable
    where TKey : IEquatable<TKey>
{
    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}