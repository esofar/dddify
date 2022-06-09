using Dddify.Auditing;
using System;

namespace Dddify.Domain.Entities;

public abstract class FullAuditedEntity<TKey> : CreationAuditedEntity<TKey>, IFullAudited
    where TKey : IEquatable<TKey>
{
    public Guid? LastModifiedBy { get; set; }

    public DateTimeOffset? LastModifiedAt { get; set; }
}
