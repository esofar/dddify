using Dddify.Exceptions;

namespace Todolist.Domain.Exceptions;

public class TodoListItemDuplicateException : DomainException
{
    public override string Name => "todolist_item_duplicate";

    public TodoListItemDuplicateException(string note)
        : base($"TodoList item with note '{note}' already exists.")
    {
    }
}
