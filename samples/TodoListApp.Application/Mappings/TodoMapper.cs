namespace TodoListApp.Application.Mappings;

public class TodoMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Todo, TodoDto>()
            .Map(dest => dest.Priority, src => src.Priority.GetDescription());
    }
}