using Dddify.Exceptions;

namespace Todolist.Domain.Exceptions;

public class TodoListItemNotFoundException : DomainException
{
    public override string Name => "todolist_item_not_found";

    public TodoListItemNotFoundException(Guid id)
        : base($"TodoList item with id '{id}' was not found.")
    {
    }
}