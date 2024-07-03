using Dddify.Domain;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Domain.Events;

public class TodoItemCreatedEvent(TodoItem todoItem) : IDomainEvent
{
    public TodoItem TodoItem => todoItem;
}
