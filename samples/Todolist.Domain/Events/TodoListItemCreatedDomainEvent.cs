using Dddify.Domain;
using Todolist.Domain.Entities;

namespace Todolist.Domain.Events;

public record TodoListItemCreatedDomainEvent : IDomainEvent
{
    public TodoListItem TodoListItem { get; private set; }

    public TodoListItemCreatedDomainEvent(TodoListItem todoListItem)
    {
        TodoListItem = todoListItem;
    }
}