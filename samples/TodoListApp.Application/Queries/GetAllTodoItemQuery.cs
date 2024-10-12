using TodoListApp.Application.Dtos;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Queries;

public record GetAllTodoItemQuery() : IQuery<IEnumerable<TodoItemDto>>;

public class GetAllTodoItemQueryHandler(ITodoItemRepository todoItemRepository) : IQueryHandler<GetAllTodoItemQuery, IEnumerable<TodoItemDto>>
{
    public async Task<IEnumerable<TodoItemDto>> Handle(GetAllTodoItemQuery query, CancellationToken cancellationToken)
    {
        return await todoItemRepository
            .AsQueryable()
            .AsNoTracking()
            .OrderBy(c => c.IsDone)
            .ThenBy(c => c.CreatedAt)
            .ProjectToType<TodoItemDto>()
            .ToListAsync(cancellationToken);
    }
}