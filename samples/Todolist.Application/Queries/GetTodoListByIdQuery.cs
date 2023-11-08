namespace Todolist.Application.Queries;

public record GetTodoListByIdQuery(Guid Id) : IQuery<TodoListDto>;

public class GetTodoQueryHandler : IQueryHandler<GetTodoListByIdQuery, TodoListDto>
{
    private readonly IApplicationDbContext _context;

    public GetTodoQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoListDto> Handle(GetTodoListByIdQuery query, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == query.Id, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(query.Id);
        }

        return todoList.Adapt<TodoListDto>();
    }
}
