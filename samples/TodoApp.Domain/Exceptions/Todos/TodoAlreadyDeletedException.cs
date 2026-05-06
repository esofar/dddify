namespace TodoApp.Domain.Exceptions.Todos;

public class TodoAlreadyDeletedException : DomainException
{
    public TodoAlreadyDeletedException(Guid id)
    {
        WithErrorCode("todo_already_deleted");
        WithMetadata("Id", id);
    }
}