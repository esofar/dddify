using TodoApp.Application.Exceptions.Todos;
using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Application.Commands.Todos;

public record UpdateTodoCommand(
    Guid Id,
    string Title,
    string? Description,
    TodoPriority Priority,
    DateTime? DueDate) : ICommand;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();

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

public class UpdateTodoCommandHandler(ITodoRepository todoRepository) : ICommandHandler<UpdateTodoCommand>
{
    public async Task Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new TodoNotFoundException(command.Id);

        todo.UpdateDetails(command.Title, command.Description, command.Priority, command.DueDate);
    }
}
