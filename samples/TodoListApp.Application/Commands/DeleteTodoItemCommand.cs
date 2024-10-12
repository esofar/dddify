using TodoListApp.Domain.Exceptions;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Commands;

public record DeleteTodoItemCommand(Guid Id) : ICommand;

public class DeleteTodoItemCommandValidator : AbstractValidator<DeleteTodoItemCommand>
{
    public DeleteTodoItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteTodoItemCommandHandler(ITodoItemRepository todoItemRepository) : ICommandHandler<DeleteTodoItemCommand>
{
    public async Task Handle(DeleteTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await todoItemRepository.GetAsync(command.Id, cancellationToken);

        if (todoItem is null)
        {
            throw new TodoItemNotFoundException(command.Id);
        }

        await todoItemRepository.RemoveAsync(todoItem);
    }
}