namespace Dddify.Domain;

/// <summary>
/// Represents base class for <see cref="IEntity"/>.
/// </summary>
public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }

    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the collection of domain events that have occurred within the aggregate.
    /// Domain events represent significant changes in the state of the aggregate that may need to be handled by other parts of the system.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate's domain events collection.
    /// This method is used to register significant state changes that should be communicated to other parts of the system.
    /// </summary>
    /// <param name="event">The domain event to add.</param>
    public void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    /// <summary>
    /// Clears all domain events from the aggregate's domain events collection.
    /// This method is typically called after the events have been processed, ensuring that no old events are left for subsequent operations.
    /// </summary>
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