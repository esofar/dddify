using Mapster;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Application.Dtos;

public class TodoItemDto
{
    public Guid Id { get; set; }

    public string Text { get; set; }

    public string PriorityLevel { get; set; }

    public bool IsDone { get; set; }
}

public class TodoItemMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<TodoItem, TodoItemDto>()
            .Map(dest => dest.PriorityLevel, src => src.PriorityLevel.GetDescription());
    }
}
