namespace Dddify.SharedKernel;

/// <summary>
/// Represents the base type for aggregate roots with a single strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the domain events currently raised by the aggregate root.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Registers a domain event on the aggregate root.
    /// </summary>
    /// <param name="event">The domain event to register.</param>
    public void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    /// <summary>
    /// Clears all domain events currently registered on the aggregate root.
    /// </summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
