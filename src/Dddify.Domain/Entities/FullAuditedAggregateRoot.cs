using System;
using Dddify.Auditing;

namespace Dddify.Domain.Entities;

[Serializable]
public abstract class FullAuditedAggregateRoot<TKey> : CreationAuditedAggregateRoot<TKey>, IFullAudited
    where TKey : IEquatable<TKey>
{
    public Guid? LastModifiedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }
}
