namespace Dddify.Domain.Entities;

public abstract class CreationAuditedEntity<TKey> : Entity<TKey>, ICreationAudited
    where TKey : IEquatable<TKey>
{
    public Guid? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }
}