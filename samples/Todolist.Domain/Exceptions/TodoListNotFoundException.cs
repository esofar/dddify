using Dddify.Exceptions;

namespace Todolist.Domain.Exceptions;

public class TodoListNotFoundException : DomainException
{
    public override string Name => "todolist_not_found";

    public TodoListNotFoundException(Guid id)
        : base($"TodoList with id '{id}' was not found.")
    {
    }
}