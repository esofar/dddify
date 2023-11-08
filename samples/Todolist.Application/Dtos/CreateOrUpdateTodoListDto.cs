namespace Todolist.Application.Dtos;

public class CreateOrUpdateTodoListDto
{
    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 主题色
    /// </summary>
    public TodoListColour Colour { get; set; }
}
