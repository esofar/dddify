namespace Dddify.Domain.Entities;

public abstract class FullAuditedEntity<TKey> : CreationAuditedEntity<TKey>, IFullAudited
    where TKey : IEquatable<TKey>
{
    public Guid? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
}