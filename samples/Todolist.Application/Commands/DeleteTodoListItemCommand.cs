namespace Todolist.Application.Commands;

public record DeleteTodoListItemCommand(Guid TodoListId, Guid TodoListItemId) : ICommand;

public class DeleteTodoListItemCommandValidator : AbstractValidator<DeleteTodoListItemCommand>
{
    public DeleteTodoListItemCommandValidator()
    {
        RuleFor(c => c.TodoListId).NotEmpty();
        RuleFor(c => c.TodoListItemId).NotEmpty();
    }
}

public class DeleteTodoListItemCommandHandler : ICommandHandler<DeleteTodoListItemCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoListItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoListItemCommand command, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == command.TodoListId, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(command.TodoListId);
        }

        todoList.DeleteItem(command.TodoListItemId);

        await _context.SaveChangesAsync(cancellationToken);
    }
}