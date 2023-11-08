namespace Todolist.Application.Dtos;

public record SortTodoListItemDto
{
    /// <summary>
    /// 排序后的待办事项ID集合
    /// </summary>
    public Guid[] TodoListItemIds { get; set; }
}