using MediatR;

namespace Todolist.Application.Commands;

public record CreateTodoListCommand(string Title, TodoListColour Colour) : ICommand;

public class CreateTodoListCommandValidator : AbstractValidator<CreateTodoListCommand>
{
    public CreateTodoListCommandValidator()
    {
        RuleFor(v => v.Title).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Colour).NotEmpty().Must(colour => TodoListColour.SupportedColours.Any(c => c.Code == colour.Code));
    }
}

public class CreateTodoListCommandHandler : ICommandHandler<CreateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateTodoListCommand command, CancellationToken cancellationToken)
    {
        if (await _context.TodoLists.AnyAsync(c => c.Title == command.Title, cancellationToken))
        {
            throw new TodoListDuplicateException(command.Title);
        }

        var todoList = new TodoList(command.Title, command.Colour);

        await _context.TodoLists.AddAsync(todoList, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
