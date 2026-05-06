namespace TodoApp.Domain.Events.Todos;

public record TodoCreatedDomainEvent(Guid Id, string Title) : IDomainEvent;