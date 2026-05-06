using TodoApp.Application.Exceptions.Todos;

namespace TodoApp.Application.Commands.Todos;

public record ReopenTodoCommand(Guid Id) : ICommand;

public class ReopenTodoCommandValidator : AbstractValidator<ReopenTodoCommand>
{
    public ReopenTodoCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class ReopenTodoCommandHandler(ITodoRepository todoRepository) : ICommandHandler<ReopenTodoCommand>
{
    public async Task Handle(ReopenTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new TodoNotFoundException(command.Id);

        todo.Reopen();
    }
}
