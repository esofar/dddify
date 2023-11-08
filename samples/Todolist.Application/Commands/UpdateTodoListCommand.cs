namespace Todolist.Application.Commands;

public record UpdateTodoListCommand(Guid Id, string Title, TodoListColour Colour) : ICommand;

public class UpdateTodoListCommandValidator : AbstractValidator<UpdateTodoListCommand>
{
    public UpdateTodoListCommandValidator()
    {
        RuleFor(c => c.Id).Empty();
        RuleFor(v => v.Title).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Colour).NotEmpty().Must(colour => TodoListColour.SupportedColours.Any(c => c.Code == colour.Code));
    }
}

public class UpdateTodoListCommandHandler : ICommandHandler<UpdateTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoListCommand command, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(command.Id);
        }

        todoList.Update(command.Title, command.Colour);

        await _context.SaveChangesAsync(cancellationToken);
    }
}