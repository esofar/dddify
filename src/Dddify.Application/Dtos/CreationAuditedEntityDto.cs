namespace Dddify.Application.Dtos;

public abstract class CreationAuditedEntityDto<TKey> : EntityDto<TKey>
{
    public Guid? CreatorId { get; }

    public string? CreatorName { get; }

    public DateTimeOffset? CreationTime { get; set; }
}
