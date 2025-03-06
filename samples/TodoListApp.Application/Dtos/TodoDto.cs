namespace TodoListApp.Application.Dtos;

public class TodoDto
{
    public Guid Id { get; set; }

    public string Text { get; set; }

    public string Priority { get; set; }

    public bool IsDone { get; set; }
}