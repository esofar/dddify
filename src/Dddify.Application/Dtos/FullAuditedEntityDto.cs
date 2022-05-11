using System;

namespace Dddify.Application.Dtos;

public abstract class FullAuditedEntityDto<TKey> : CreationAuditedEntityDto<TKey>
{
    public Guid? LastModifierId { get; }

    public string? LastModifierName { get; }

    public DateTimeOffset? LastModificationTime { get; set; }
}
