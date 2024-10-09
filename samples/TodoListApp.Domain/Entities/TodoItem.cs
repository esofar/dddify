using Dddify.Domain;
using TodoListApp.Domain.Enums;
using TodoListApp.Domain.DomainEvents;

namespace TodoListApp.Domain.Entities;

public class TodoItem : FullAuditableAggregateRoot<Guid>
{
    public TodoItem(string text, PriorityLevel priorityLevel)
    {
        Text = text;
        PriorityLevel = priorityLevel;

        AddDomainEvent(new TodoItemCreatedDomainEvent(this));
    }

    private TodoItem() { }

    public string Text { get; private set; }

    public PriorityLevel PriorityLevel { get; private set; }

    public bool IsDone { get; private set; }

    public void Complete(bool isDone)
    {
        IsDone = isDone;
        AddDomainEvent(new TodoItemCompletedDomainEvent(this));
    }
}
