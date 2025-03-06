using Dddify.Domain;
using TodoListApp.Domain.Events;

namespace TodoListApp.Domain.Entities;

public class Todo : FullAuditableAggregateRoot<Guid>
{
    public string Text { get; private set; }

    public TodoPriority Priority { get; private set; }

    public bool IsDone { get; private set; }

    private Todo() { }

    public Todo(Guid id, string text, TodoPriority priority)
    {
        Id = id;
        Text = text;
        Priority = priority;
        AddDomainEvent(new TodoCreatedDomainEvent(Id, Text));
    }

    public void Update(bool isDone)
    {
        IsDone = isDone;
        AddDomainEvent(new TodoUpdatedDomainEvent(Id, IsDone));
    }
}
