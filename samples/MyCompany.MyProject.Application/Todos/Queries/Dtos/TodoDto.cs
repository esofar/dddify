using Mapster;
using MyCompany.MyProject.Domain.Entities;

namespace MyCompany.MyProject.Application.Todos.Queries;

public class TodoDto
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string ColourCode { get; set; }
}

public class TodoMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<Todo, TodoDto>()
            .Map(dest => dest.Title, src => src.Title + "_AppendSomething!");
    }
}
