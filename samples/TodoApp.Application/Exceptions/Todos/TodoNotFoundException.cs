using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Application.Exceptions.Todos;

public class TodoNotFoundException : AppException
{
    public TodoNotFoundException(Guid id)
    {
        WithErrorCode("todo_not_found");
        WithMetadata(nameof(Todo.Id), id);
    }
}
