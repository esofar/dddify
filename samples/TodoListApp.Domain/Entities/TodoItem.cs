using Dddify.Domain.Entities;
using TodoListApp.Domain.Enums;
using TodoListApp.Domain.Events;

namespace TodoListApp.Domain.Entities;

public class TodoItem : FullAuditedAggregateRoot<Guid>
{
    public TodoItem(string text, PriorityLevel priorityLevel)
    {
        Text = text;
        PriorityLevel = priorityLevel;
        AddDomainEvent(new TodoItemCreatedEvent(this));
    }

    private TodoItem() { }

    public string Text { get; private set; }

    public PriorityLevel PriorityLevel { get; private set; }

    public bool IsDone { get; private set; }

    public void Complete(bool isDone)
    {
        IsDone = isDone;
        AddDomainEvent(new TodoItemCompletedEvent(this));
    }
}
