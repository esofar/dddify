using Dddify.Exceptions;

namespace TodoListApp.Domain.Exceptions;

public class TodoItemNotFoundException(Guid id) : DomainException($"Todo item with '{id}' was not found.")
{
    public override string Name => "Todo item was not found.";
}
