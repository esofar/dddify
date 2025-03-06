using Dddify.Exceptions;

namespace TodoListApp.Domain.Exceptions;

public class TodoDuplicateException(string text) 
    : DomainException($"Todo '{text}' already exists.")
{
    public override string LocalizedName => $"Todo already exists.";
}