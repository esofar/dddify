namespace Todolist.Application.Commands;

public record CreateTodoListItemCommand(
    Guid TodoListId,
    string Note,
    string Priority) : ICommand;

public class CreateTodoListItemCommandValidator : AbstractValidator<CreateTodoListItemCommand>
{
    public CreateTodoListItemCommandValidator()
    {
        RuleFor(c => c.TodoListId).NotEmpty();
        RuleFor(c => c.Note).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Priority).NotEmpty().IsEnumName(typeof(TodoListItemPriority), false);
    }
}

public class CreateTodoListItemCommandHandler : ICommandHandler<CreateTodoListItemCommand>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CreateTodoListItemCommand command, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.TodoListId, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(command.TodoListId);
        }

        todoList.AddItem(
            command.Note,
            command.Priority.ToEnum<TodoListItemPriority>());

        await _context.SaveChangesAsync(cancellationToken);
    }
}