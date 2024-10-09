using Dddify.Domain;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Domain.DomainEvents;

public class TodoItemCompletedDomainEvent(TodoItem todoItem) : IDomainEvent
{
    public TodoItem TodoItem => todoItem;
}
