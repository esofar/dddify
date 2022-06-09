using Dddify.Auditing;
using System;

namespace Dddify.Domain.Entities;

[Serializable]
public abstract class CreationAuditedAggregateRoot<TKey> : AggregateRoot<TKey>, ICreationAudited
    where TKey : IEquatable<TKey>
{
    public Guid? CreatedBy { get; set; }

    public DateTimeOffset? CreatedAt { get; set; }
}
