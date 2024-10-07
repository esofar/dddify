namespace Dddify.Domain;

/// <summary>
/// Represents a fully auditable aggregate.
/// </summary>
/// <remarks>
/// This class inherits from <see cref="AggregateRoot{TKey}"/> and implements <see cref="IFullAuditable"/> to provide audit properties for the aggregate.
/// </remarks>
/// <typeparam name="TKey">The type of the primary key for the aggregate root.</typeparam>
public abstract class FullAuditableAggregateRoot<TKey> :  AggregateRoot<TKey>, IFullAuditable
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