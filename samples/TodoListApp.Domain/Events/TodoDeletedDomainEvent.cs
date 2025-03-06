using Dddify.Domain;

namespace TodoListApp.Domain.Events;

public record TodoDeletedDomainEvent(Guid TodoId) : IDomainEvent;