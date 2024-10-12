using Dddify.Domain;
using TodoListApp.Domain.Enums;
using TodoListApp.Domain.Events;

namespace TodoListApp.Domain.Entities;

public class TodoItem : FullAuditableAggregateRoot<Guid>
{
    public string Text { get; private set; }

    public PriorityLevel PriorityLevel { get; private set; }

    public bool IsDone { get; private set; }

    private TodoItem() { }

    public TodoItem(string text, PriorityLevel priorityLevel)
    {
        Text = text;
        PriorityLevel = priorityLevel;

        AddDomainEvent(new TodoItemCreatedEvent(Text));
    }

    public void MarkAs(bool isDone)
    {
        IsDone = isDone;
        AddDomainEvent(new TodoItemMarkedEvent(Id, IsDone));
    }
}
