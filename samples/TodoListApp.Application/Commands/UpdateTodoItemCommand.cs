using TodoListApp.Domain.Exceptions;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Commands;

public record UpdateTodoItemCommand(Guid Id, bool IsDone) : ICommand;

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class UpdateTodoItemCommandHandler(ITodoItemRepository todoItemRepository) : ICommandHandler<UpdateTodoItemCommand>
{
    public async Task Handle(UpdateTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await todoItemRepository.GetAsync(command.Id, cancellationToken);

        if (todoItem is null)
        {
            throw new TodoItemNotFoundException(command.Id);
        }

        todoItem.MarkAs(command.IsDone);
    }
}
