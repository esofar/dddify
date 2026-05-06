namespace TodoApp.Application.Dtos.Todos;

public sealed record TodoStatsDto(
    int TotalCount,
    int ActiveCount,
    int CompletedCount,
    int DueTodayCount);
