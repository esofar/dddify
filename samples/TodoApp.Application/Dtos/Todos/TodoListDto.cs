using TodoApp.Application.Queries.Todos;

namespace TodoApp.Application.Dtos.Todos;

public sealed record TodoListDto(
    IReadOnlyList<TodoDto> Items,
    TodoStatsDto Stats,
    TodoListFilter Filter,
    string? SearchTerm);
