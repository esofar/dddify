namespace Todolist.Application.Dtos;

public record TodoListDto
{
    /// <summary>
    /// 待办ID
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 主题色代码
    /// </summary>
    public string ColourCode { get; set; }
}

public class TodoMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<TodoList, TodoListDto>()
            //.Map(dest => dest.Title, src => $"✔ {src.Title}")
            .Map(dest => dest.ColourCode, src => src.Colour.Code);
    }
}
