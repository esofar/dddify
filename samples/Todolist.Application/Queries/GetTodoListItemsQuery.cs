namespace Todolist.Application.Queries;

public record GetTodoListItemsQuery(Guid TodoListId) : IQuery<IEnumerable<TodoListItemDto>>;

public class GetTodoListItemsQueryHandler : IQueryHandler<GetTodoListItemsQuery, IEnumerable<TodoListItemDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoListItemsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoListItemDto>> Handle(GetTodoListItemsQuery query, CancellationToken cancellationToken)
    {
        var todoList = await _context.TodoLists
           .Include(c => c.Items.OrderBy(x => x.Order))
           .FirstOrDefaultAsync(c => c.Id == query.TodoListId, cancellationToken);

        if (todoList is null)
        {
            throw new TodoListNotFoundException(query.TodoListId);
        }

        return _mapper.Map<IEnumerable<TodoListItemDto>>(todoList.Items);
    }
}