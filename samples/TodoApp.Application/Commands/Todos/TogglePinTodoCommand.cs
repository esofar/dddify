using TodoApp.Application.Exceptions.Todos;

namespace TodoApp.Application.Commands.Todos;

public record TogglePinTodoCommand(Guid Id) : ICommand;

public class TogglePinTodoCommandValidator : AbstractValidator<TogglePinTodoCommand>
{
    public TogglePinTodoCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class TogglePinTodoCommandHandler(ITodoRepository todoRepository) : ICommandHandler<TogglePinTodoCommand>
{
    public async Task Handle(TogglePinTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new TodoNotFoundException(command.Id);

        if (todo.IsPinned)
        {
            todo.Unpin();
            return;
        }

        todo.Pin();
    }
}
