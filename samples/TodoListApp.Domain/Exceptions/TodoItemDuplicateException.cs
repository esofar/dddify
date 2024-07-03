using Dddify.Exceptions;

namespace TodoListApp.Domain.Exceptions;

public class TodoItemDuplicateException(string text) : DomainException($"Todo item '{text}' already exists.")
{
    public override string Name => "Todo item already exists.";
}
