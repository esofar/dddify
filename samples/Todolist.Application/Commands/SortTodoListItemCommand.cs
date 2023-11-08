namespace Todolist.Application.Commands;

public record SortTodoListItemCommand(Guid TodoListId, Guid[] TodoListItemIds) : ICommand;

public class SortTodoListItemCommandValidator : AbstractValidator<SortTodoListItemCommand>
{
    public SortTodoListItemCommandValidator()
    {
        RuleFor(c => c.TodoListId).NotEmpty();
        RuleFor(c => c.TodoListItemIds).NotEmpty();
    }
}

public class SortTodoListItemCommandHandler : ICommandHandler<SortTodoListItemCommand>
{
    private readonly IApplicationDbContext _context;

    public SortTodoListItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SortTodoListItemCommand command, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.TodoListId, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(command.TodoListId);
        }

        todoList.SortItems(command.TodoListItemIds);

        await _context.SaveChangesAsync(cancellationToken);
    }
}