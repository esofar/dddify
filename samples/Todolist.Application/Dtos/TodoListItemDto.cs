namespace Todolist.Application.Dtos;

public record TodoListItemDto
{
    /// <summary>
    /// 待办事项ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    public string Note { get; set; }

    /// <summary>
    /// 优先级
    /// </summary>
    public string Priority { get; set; }
}

public class TodoItemMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<TodoListItem, TodoListItemDto>()
            .Map(dest => dest.Priority, src => src.Priority.GetDescription());
    }
}
