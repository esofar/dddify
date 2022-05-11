using System;

namespace Dddify.Application.Dtos;

[Serializable]
public abstract class EntityDto : IEntityDto
{
    public override string ToString()
    {
        return $"[Dto: {GetType().Name}]";
    }
}

[Serializable]
public abstract class EntityDto<TKey> : EntityDto, IEntityDto<TKey>
{
    /// <summary>
    /// Id of the entity.
    /// </summary>
    public TKey Id { get; set; }

    public override string ToString()
    {
        return $"[Dto: {GetType().Name}] Id = {Id}";
    }
}

