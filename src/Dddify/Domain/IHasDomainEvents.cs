namespace Dddify.Domain;

/// <summary>
/// Represents an entity as capable of containing domain events.
/// </summary>
public interface IHasDomainEvents
{
    /// <summary>
    /// Gets the collection of domain events that have occurred within the aggregate.
    /// </summary>
    /// <remarks>
    /// Domain events represent significant changes in the state of the aggregate that may need to be handled by other parts of the system.
    /// </remarks>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event to the aggregate's domain events collection.
    /// </summary>
    /// <remarks>
    /// This method is used to register significant state changes that should be communicated to other parts of the system.
    /// </remarks>
    /// <param name="event">The domain event to add.</param>
    void AddDomainEvent(IDomainEvent @event);

    /// <summary>
    /// Clears all domain events from the aggregate's domain events collection.
    /// </summary>
    /// <remarks>
    /// This method is typically called after the events have been processed, ensuring that no old events are left for subsequent operations.
    /// </remarks>
    void ClearDomainEvents();
}