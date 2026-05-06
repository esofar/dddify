using MapsterMapper;
using TodoApp.Application.Dtos.Todos;
using TodoApp.Application.Exceptions.Todos;

namespace TodoApp.Application.Queries.Todos;

public sealed record GetTodoByIdQuery(Guid Id) : IQuery<TodoDto>;

public sealed class GetTodoByIdQueryHandler(ITodoRepository todoRepository, IMapper mapper) : IQueryHandler<GetTodoByIdQuery, TodoDto>
{
    public async Task<TodoDto> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(query.Id, cancellationToken)
            ?? throw new TodoNotFoundException(query.Id);

        return new TodoDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.Priority,
            todo.Status,
            todo.DueDate,
            todo.IsPinned,
            todo.CompletedAt);
    }
}
