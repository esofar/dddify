using Dddify.Domain;
using Todolist.Domain.Entities;

namespace Todolist.Domain.Events;

public record TodoListItemCompletedDomainEvent : IDomainEvent
{
    public TodoListItem TodoListItem { get; private set; }

    public TodoListItemCompletedDomainEvent(TodoListItem todoListItem)
    {
        TodoListItem = todoListItem;
    }
}