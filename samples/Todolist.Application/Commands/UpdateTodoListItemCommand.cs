namespace Todolist.Application.Commands;

public record UpdateTodoListItemCommand(
    Guid TodoListId,
    Guid TodoListItemId,
    string Note,
    string Priority) : ICommand;

public class UpdateTodoListItemCommandValidator : AbstractValidator<UpdateTodoListItemCommand>
{
    public UpdateTodoListItemCommandValidator()
    {
        RuleFor(c => c.TodoListId).NotEmpty();
        RuleFor(c => c.TodoListItemId).NotEmpty();
        RuleFor(c => c.Note).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Priority).NotEmpty().IsEnumName(typeof(TodoListItemPriority));
    }
}

public class UpdateTodoListItemCommandHandler : ICommandHandler<UpdateTodoListItemCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateTodoListItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateTodoListItemCommand command, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.TodoListId, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(command.TodoListId);
        }

        todoList.UpdateItem(
            command.TodoListItemId,
            command.Note,
            command.Priority.ToEnum<TodoListItemPriority>());

        await _context.SaveChangesAsync(cancellationToken);
    }
}