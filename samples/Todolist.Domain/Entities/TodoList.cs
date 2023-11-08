using Dddify.Domain;
using Dddify.Domain.Entities;
using Todolist.Domain.Events;
using Todolist.Domain.Exceptions;

namespace Todolist.Domain.Entities;

public class TodoList : FullAuditedAggregateRoot<Guid>, ISoftDeletable
{
    private readonly List<TodoListItem> _items = new();

    public TodoList(string title, TodoListColour colour)
    {
        Title = title;
        Colour = colour;

        AddDomainEvent(new TodoListCreatedDomainEvent(this));
    }

    private TodoList() { }

    public string Title { get; private set; }

    public TodoListColour Colour { get; private set; }

    public bool IsDeleted { get; set; }

    public IReadOnlyCollection<TodoListItem> Items => _items.AsReadOnly();

    public void Update(string title, TodoListColour colour)
    {
        Title = title;
        Colour = colour;
    }

    public void AddItem(string note, TodoListItemPriority priority)
    {
        if (_items.Any(c => c.Note == note))
        {
            throw new TodoListItemDuplicateException(note);
        }

        var item = new TodoListItem(note, priority);

        _items.Add(item);

        AddDomainEvent(new TodoListItemCreatedDomainEvent(item));
    }

    public void UpdateItem(Guid id, string note, TodoListItemPriority priority)
    {
        var item = _items.FirstOrDefault(c => c.Id == id);

        if (item is null)
        {
            throw new TodoListItemNotFoundException(id);
        }

        if (_items.Any(c => c.Id != id && c.Note == note))
        {
            throw new TodoListItemDuplicateException(note);
        }

        item.Update(note, priority);
    }

    public void DeleteItem(Guid id)
    {
        var item = _items.SingleOrDefault(c => c.Id == id);

        if (item is null)
        {
            throw new TodoListItemNotFoundException(id);
        }

        _items.Remove(item);
    }

    public void SortItems(Guid[] orderedIds)
    {
        for (int order = 1; order <= orderedIds.Length; order++)
        {
            _items.Single(c => c.Id == orderedIds[order]).Sort(order);
        }
    }
}
