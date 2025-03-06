using Dddify.Domain;

namespace TodoListApp.Domain.Events;

public record TodoCreatedDomainEvent(Guid TodoId, string Text) : IDomainEvent;