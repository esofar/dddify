using Dddify.Exceptions;

namespace TodoListApp.Domain.Exceptions;

public class TodoNotFoundException(Guid id) 
    : DomainException($"Todo with id '{id}' was not found.")
{
    public override string LocalizedName => "Todo was not found.";
}