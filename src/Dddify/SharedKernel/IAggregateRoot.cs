namespace Dddify.SharedKernel;

/// <summary>
/// Represents an aggregate root in the domain model.
/// </summary>
/// <remarks>
/// This non-generic contract is intended for aggregate roots whose key is not exposed as a single
/// <c>Id</c> property or whose identity consists of multiple values.
/// </remarks>
public interface IAggregateRoot : IEntity
{
    /// <summary>
    /// Gets the domain events raised by the aggregate during the current unit of work.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Adds a domain event raised by the aggregate.
    /// </summary>
    /// <param name="event">The domain event to register.</param>
    void AddDomainEvent(IDomainEvent @event);

    /// <summary>
    /// Clears all domain events currently registered on the aggregate.
    /// </summary>
    void ClearDomainEvents();
}

/// <summary>
/// Represents an aggregate root with a strongly typed single-key identity.
/// </summary>
/// <typeparam name="TKey">The type of the aggregate root identifier.</typeparam>
public interface IAggregateRoot<TKey> : IEntity<TKey>, IAggregateRoot
{
}
