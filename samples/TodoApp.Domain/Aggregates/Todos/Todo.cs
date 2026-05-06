using TodoApp.Domain.Events.Todos;
using TodoApp.Domain.Exceptions.Todos;

namespace TodoApp.Domain.Aggregates.Todos;

public class Todo : AggregateRoot<Guid>, ISoftDeletable
{
    public const int TitleMaxLength = 120;
    public const int DescriptionMaxLength = 600;

    private Todo() { }

    public Todo(Guid id, string title, string? description, TodoPriority priority, DateTime? dueDate)
    {
        Id = id;
        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate?.Date;
        Status = TodoStatus.Active;

        AddDomainEvent(new TodoCreatedDomainEvent(id, title));
    }

    public string Title { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public bool IsPinned { get; private set; }

    public TodoPriority Priority { get; private set; } = TodoPriority.Medium;

    public TodoStatus Status { get; private set; } = TodoStatus.Active;

    public DateTime? DueDate { get; private set; }

    public DateTime? CompletedAt { get; private set; }

    public bool IsDeleted { get; set; }

    public void UpdateDetails(string title, string? description, TodoPriority priority, DateTime? dueDate)
    {
        EnsureNotDeleted();

        Title = title;
        Description = description;
        Priority = priority;
        DueDate = dueDate?.Date;

        AddDomainEvent(new TodoUpdatedDomainEvent(Id));
    }

    public void Complete(DateTime completedAt)
    {
        EnsureNotDeleted();

        if (Status == TodoStatus.Completed)
        {
            return;
        }

        Status = TodoStatus.Completed;
        CompletedAt = completedAt;
    }

    public void Reopen()
    {
        EnsureNotDeleted();

        if (Status == TodoStatus.Active)
        {
            return;
        }

        Status = TodoStatus.Active;
        CompletedAt = null;
    }

    public void Pin()
    {
        EnsureNotDeleted();
        IsPinned = true;
    }

    public void Unpin()
    {
        EnsureNotDeleted();
        IsPinned = false;
    }

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
        {
            throw new TodoAlreadyDeletedException(Id);
        }
    }
}
