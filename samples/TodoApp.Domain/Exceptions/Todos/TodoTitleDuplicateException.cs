using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Domain.Exceptions.Todos;

public class TodoTitleDuplicateException : DomainException
{
    public TodoTitleDuplicateException(string title)
    {
        WithErrorCode("todo_title_duplicate");
        WithMetadata(nameof(Todo.Title), title);
    }
}