using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Application.Dtos.Todos;

public sealed record TodoDto(
    Guid Id,
    string Title,
    string? Description,
    TodoPriority Priority,
    TodoStatus Status,
    DateTime? DueDate,
    bool IsPinned,
    DateTime? CompletedAt);
