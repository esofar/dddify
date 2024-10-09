namespace Dddify.Domain;

/// <summary>
/// Represents base class for <see cref="IEntity"/>.
/// </summary>
public abstract class Entity : IEntity, IHasDomainEvents
{
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }

    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}

/// <summary>
/// Represents base class for <see cref="IEntity{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the primary key for the entity.</typeparam>
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    private int? _requestedHashCode;

    public virtual TKey Id { get; set; } = default!;

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id!.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }
        else
        {
            return base.GetHashCode();
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is Entity<TKey> entity && entity.IsTransient() && !IsTransient() && entity.Id!.Equals(Id);
    }

    public override object[] GetKeys()
    {
        return [Id!];
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
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }

    public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
    {
        return !(left == right);
    }
}