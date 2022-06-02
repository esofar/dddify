using Mapster;
using MyCompany.MyProject.Domain.Entities;

namespace MyCompany.MyProject.Application.Todos.Queries;

public record TodoDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public string ColourCode { get; set; } = default!;

    public string ConcurrencyStamp { get; set; } = default!;
}

public class TodoMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Todo, TodoDto>()
            .Map(dest => dest.Title, src => $"✔ {src.Title}");
    }
}
