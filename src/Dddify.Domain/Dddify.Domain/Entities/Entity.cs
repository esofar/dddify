using Dddify.Domain.Events;
using System;
using System.Collections.Generic;

namespace Dddify.Domain.Entities;

[Serializable]
public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Keys = { string.Join(", ", GetKeys()) }";
    }

    private List<IDomainEvent> _domainEvents;

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents ??= new List<IDomainEvent>();
        _domainEvents.Add(@event);
    }

    public void RemoveDomainEvent(IDomainEvent @event)
    {
        _domainEvents?.Remove(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents?.Clear();
    }
}

[Serializable]
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    int? _requestedHashCode;

    public virtual TKey Id { get; set; }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }
        else
        {
            return base.GetHashCode();
        }
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity<TKey>))
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var other = obj as Entity<TKey>;

        return !other.IsTransient() && !IsTransient() && other.Id.Equals(Id);
    }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Id = {Id}";
    }

    public bool IsTransient()
    {
        return EqualityComparer<TKey>.Default.Equals(Id, default);
    }

    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
    {
        if (Equals(left, null))
        {
            return Equals(right, null) ? true : false;
        }
        else
        {
            return left.Equals(right);
        }
    }

    public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
    {
        return !(left == right);
    }
}
