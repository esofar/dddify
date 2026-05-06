using TodoApp.Application.Dtos.Todos;
using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Application.Queries.Todos;

public sealed record GetTodoListQuery(TodoListFilter Filter = TodoListFilter.All, string? SearchTerm = null) : IQuery<TodoListDto>;

public enum TodoListFilter
{
    All = 1,
    Active = 2,
    Completed = 3,
    DueToday = 4
}

public sealed class GetTodoListQueryHandler(ITodoRepository todoRepository, IClock clock) : IQueryHandler<GetTodoListQuery, TodoListDto>
{
    public async Task<TodoListDto> Handle(GetTodoListQuery query, CancellationToken cancellationToken)
    {
        var today = clock.Now.Date;
        var normalizedSearch = string.IsNullOrWhiteSpace(query.SearchTerm) ? null : query.SearchTerm.Trim();

        var baseQuery = todoRepository.AsQueryable().AsNoTracking();
        var filteredQuery = baseQuery
            .WhereIf(!string.IsNullOrWhiteSpace(normalizedSearch),
                x => x.Title.Contains(normalizedSearch!) || (x.Description != null && x.Description.Contains(normalizedSearch!)));

        filteredQuery = query.Filter switch
        {
            TodoListFilter.Active => filteredQuery.Where(x => x.Status == TodoStatus.Active),
            TodoListFilter.Completed => filteredQuery.Where(x => x.Status == TodoStatus.Completed),
            TodoListFilter.DueToday => filteredQuery.Where(x => x.DueDate.HasValue && x.DueDate.Value.Date == today),
            _ => filteredQuery
        };

        var items = await filteredQuery
            .OrderByDescending(x => x.IsPinned)
            .ThenBy(x => x.Status == TodoStatus.Completed)
            .ThenBy(x => x.DueDate.HasValue ? 0 : 1)
            .ThenBy(x => x.DueDate)
            .Select(x => new TodoDto(
                x.Id,
                x.Title,
                x.Description,
                x.Priority,
                x.Status,
                x.DueDate,
                x.IsPinned,
                x.CompletedAt))
            .ToListAsync(cancellationToken);

        var stats = new TodoStatsDto(
            await baseQuery.CountAsync(cancellationToken),
            await baseQuery.CountAsync(x => x.Status == TodoStatus.Active, cancellationToken),
            await baseQuery.CountAsync(x => x.Status == TodoStatus.Completed, cancellationToken),
            await baseQuery.CountAsync(x => x.DueDate.HasValue && x.DueDate.Value.Date == today, cancellationToken));

        return new TodoListDto(items, stats, query.Filter, normalizedSearch);
    }
}

