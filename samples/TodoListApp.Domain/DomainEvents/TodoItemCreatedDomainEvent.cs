using Dddify.Domain;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Domain.DomainEvents;

public class TodoItemCreatedDomainEvent(TodoItem todoItem) : IDomainEvent
{
    public TodoItem TodoItem => todoItem;
}
