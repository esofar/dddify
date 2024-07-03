namespace Dddify.Domain.Entities;

public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Add(@event);
    }

    public void RemoveDomainEvent(IDomainEvent @event)
    {
        _domainEvents.Remove(@event);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}