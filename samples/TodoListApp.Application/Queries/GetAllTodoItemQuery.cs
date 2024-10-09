using Dddify.Messaging.Queries;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Application.Dtos;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Queries;

public record GetAllTodoItemQuery() : IQuery<IEnumerable<TodoItemDto>>;

public class GetAllTodoItemQueryHandler(ITodoItemRepository todoItemRepository) : IQueryHandler<GetAllTodoItemQuery, IEnumerable<TodoItemDto>>
{
    public async Task<IEnumerable<TodoItemDto>> Handle(GetAllTodoItemQuery query, CancellationToken cancellationToken)
    {
        return await todoItemRepository
            .AsNoTrackingQueryable()
            .OrderBy(x => x.CreatedAt)
            .ProjectToType<TodoItemDto>()
            .ToListAsync(cancellationToken);
    }
}