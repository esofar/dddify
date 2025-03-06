namespace Dddify.Domain;

/// <summary>
/// Represents base class for <see cref="IAggregateRoot{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the primary key for the aggregate root.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    public void ClearDomainEvents() => _domainEvents.Clear();
}