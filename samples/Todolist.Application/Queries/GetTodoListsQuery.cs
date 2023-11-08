namespace Todolist.Application.Queries;

public record GetTodoListsQuery(int Page, int Size) : IQuery<IPagedResult<TodoListDto>>;

public class GetTodoPagedListQueryHandler : IQueryHandler<GetTodoListsQuery, IPagedResult<TodoListDto>>
{
    private readonly IApplicationDbContext _context;

    public GetTodoPagedListQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IPagedResult<TodoListDto>> Handle(GetTodoListsQuery query, CancellationToken cancellationToken)
    {
        return await _context.TodoLists
            .AsNoTracking()
            .ProjectToType<TodoListDto>()
            .OrderBy(c => c.Title)
            .ToPagedResultAsync(query.Page, query.Size, cancellationToken);
    }
}
