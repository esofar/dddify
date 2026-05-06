using TodoApp.Application.Exceptions.Todos;

namespace TodoApp.Application.Commands.Todos;

public record CompleteTodoCommand(Guid Id) : ICommand;

public class CompleteTodoCommandValidator : AbstractValidator<CompleteTodoCommand>
{
    public CompleteTodoCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class CompleteTodoCommandHandler(ITodoRepository todoRepository, IClock clock) : ICommandHandler<CompleteTodoCommand>
{
    public async Task Handle(CompleteTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new TodoNotFoundException(command.Id);

        todo.Complete(clock.Now.DateTime);
    }
}
