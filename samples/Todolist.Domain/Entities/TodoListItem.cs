using Dddify.Domain;
using Dddify.Domain.Entities;

namespace Todolist.Domain.Entities;

public class TodoListItem : FullAuditedEntity<Guid>, ISoftDeletable
{
    public TodoListItem(string note, TodoListItemPriority priority)
    {
        Note = note;
        Priority = priority;
    }

    private TodoListItem() { }

    public string Note { get; private set; }

    public TodoListItemPriority Priority { get; private set; }

    public bool IsDone { get; private set; }

    public int Order { get; private set; }

    public TodoList TodoList { get; private set; }

    public Guid TodoListId { get; private set; }

    public bool IsDeleted { get; set; }

    public void Update(string note, TodoListItemPriority priority)
    {
        Note = note;
        Priority = priority;
    }

    public void MarkComplete()
    {
        IsDone = true;
    }

    public void Sort(int order)
        => Order = order;
}
