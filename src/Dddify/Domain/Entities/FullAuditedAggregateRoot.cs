namespace Dddify.Domain.Entities;

public abstract class FullAuditedAggregateRoot<TKey> : CreationAuditedAggregateRoot<TKey>, IFullAudited
    where TKey : IEquatable<TKey>
{
    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}