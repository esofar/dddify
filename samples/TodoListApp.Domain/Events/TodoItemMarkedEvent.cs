using Dddify.Domain;

namespace TodoListApp.Domain.Events;

public record TodoItemMarkedEvent(Guid TodoItemId, bool IsDone) : IDomainEvent;