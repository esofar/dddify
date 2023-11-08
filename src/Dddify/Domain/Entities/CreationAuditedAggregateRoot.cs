namespace Dddify.Domain.Entities;

public abstract class CreationAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAudited
    where TKey : IEquatable<TKey>
{
    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
}