using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Domain.Events.Todos;

public record TodoUpdatedDomainEvent(Guid Id) : IDomainEvent;
