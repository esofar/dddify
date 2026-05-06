namespace TodoApp.Domain.Events.Todos;

public record TodoDeletedDomainEvent(Guid Id) : IDomainEvent;