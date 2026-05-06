using TodoApp.Application.Services;
using TodoApp.Domain.Aggregates.Todos;
using TodoApp.Domain.Exceptions.Todos;

namespace TodoApp.Application.Commands.Todos;

public record CreateTodoCommand(
    string Title,
    string? Description,
    TodoPriority Priority,
    DateTime? DueDate) : ICommand<Guid>;

public class CreateTodoCommandValidator : AbstractValidator<CreateTodoCommand>
{
    public CreateTodoCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty()
            .MaximumLength(Todo.TitleMaxLength);

        RuleFor(c => c.Description)
            .MaximumLength(Todo.DescriptionMaxLength);

        RuleFor(c => c.Priority)
            .NotNull()
            .IsInEnum();

        RuleFor(x => x.DueDate)
           .Must(dueDate => dueDate is null || dueDate.Value.Date >= DateTime.Today.AddYears(-1));
    }
}

public class CreateTodoCommandHandler(ITodoRepository todoRepository, IGuidGenerator guidGenerator) : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        if (await todoRepository.AnyAsync(c => c.Title == command.Title, cancellationToken))
        {
            throw new TodoTitleDuplicateException(command.Title);
        }

        var todo = new Todo(guidGenerator.Create(), command.Title, command.Description, command.Priority, command.DueDate);

        await todoRepository.AddAsync(todo, cancellationToken);

        return todo.Id;
    }
}