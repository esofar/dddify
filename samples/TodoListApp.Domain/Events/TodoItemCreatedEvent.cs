using Dddify.Domain;

namespace TodoListApp.Domain.Events;

public record TodoItemCreatedEvent(string Text) : IDomainEvent;