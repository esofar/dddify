namespace Todolist.Application.Commands;

public record DeleteTodoListCommand(Guid Id) : ICommand;

public class DeleteTodoListCommandValidator : AbstractValidator<DeleteTodoListCommand>
{
    public DeleteTodoListCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteTodoCommandHandler : ICommandHandler<DeleteTodoListCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteTodoCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteTodoListCommand command, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(command.Id);
        }

        _context.TodoLists.Remove(todoList);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
