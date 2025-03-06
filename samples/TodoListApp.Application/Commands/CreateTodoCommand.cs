namespace TodoListApp.Application.Commands;

public record CreateTodoCommand(string Text, TodoPriority Priority) : ICommand<Guid>;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(c => c.Text).NotEmpty().MaximumLength(50);
        RuleFor(c => c.Priority).NotNull().IsInEnum();
    }
}

public class CreateTodoCommandHandler(ITodoRepository todoRepository, IGuidGenerator guidGenerator) : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        if (await todoRepository.AnyAsync(c => c.Text == command.Text, cancellationToken))
        {
            throw new TodoDuplicateException(command.Text);
        }

        var todo = new Todo(guidGenerator.Create(), command.Text, command.Priority);

        await todoRepository.AddAsync(todo, cancellationToken);

        return todo.Id;
    }
}
