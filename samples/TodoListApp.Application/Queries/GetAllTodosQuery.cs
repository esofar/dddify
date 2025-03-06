namespace TodoListApp.Application.Queries;

public record GetAllTodosQuery() : IQuery<IEnumerable<TodoDto>>;

public class GetAllTodosItemQueryHandler(ITodoRepository todoRepository) : IQueryHandler<GetAllTodosQuery, IEnumerable<TodoDto>>
{
    public async Task<IEnumerable<TodoDto>> Handle(GetAllTodosQuery query, CancellationToken cancellationToken)
    {
        var todos = await todoRepository.GetSortedTodosAsync();

        return todos.Adapt<IEnumerable<TodoDto>>();
    }
}