namespace Dddify.Domain.Entities;

public abstract class FullAuditedAggregateRoot<TKey> : CreationAuditedAggregateRoot<TKey>, IFullAudited, ISoftDeletable
    where TKey : IEquatable<TKey>
{
    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? DeletedBy { get; set; }

    public DateTime? DeletedAt { get; set; }
}