namespace Todolist.Application.Dtos;

public record CreateOrUpdateTodoListItemDto
{
    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public string Priority { get; set; }
}