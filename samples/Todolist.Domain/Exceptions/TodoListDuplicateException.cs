using Dddify.Exceptions;

namespace Todolist.Domain.Exceptions;

public class TodoListDuplicateException : DomainException
{
    public override string Name => "todolist_duplicate";

    public TodoListDuplicateException(string title)
        : base($"TodoList with title '{title}' already exists.")
    {
    }
}
