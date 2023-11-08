namespace Todolist.Application.Queries;

public record GetTodoListItemByIdQuery(
    Guid TodoListId,
    Guid TodoListItemId) : IQuery<TodoListItemDto>;

public class GetTodoListItemByIdQueryHandler : IQueryHandler<GetTodoListItemByIdQuery, TodoListItemDto>
{
    private readonly IApplicationDbContext _context;

    public GetTodoListItemByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoListItemDto> Handle(GetTodoListItemByIdQuery query, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .AsNoTracking()
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.Id == query.TodoListId, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(query.TodoListId);
        }

        var todoListItem = todoList.Items.FirstOrDefault(c => c.Id == query.TodoListItemId);

        if (todoListItem is null)
        {
            throw new TodoListItemNotFoundException(query.TodoListItemId);
        }

        return todoListItem.Adapt<TodoListItemDto>();
    }
}