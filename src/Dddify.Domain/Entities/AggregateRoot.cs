using System;

namespace Dddify.Domain.Entities;

[Serializable]
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
{
    
}
