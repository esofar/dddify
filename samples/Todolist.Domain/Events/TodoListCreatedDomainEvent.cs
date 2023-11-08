using Dddify.Domain;
using Todolist.Domain.Entities;

namespace Todolist.Domain.Events;

public record TodoListCreatedDomainEvent : IDomainEvent
{
    public TodoList TodoList { get; private set; }

    public TodoListCreatedDomainEvent(TodoList todoList)
    {
        TodoList = todoList;
    }
}
