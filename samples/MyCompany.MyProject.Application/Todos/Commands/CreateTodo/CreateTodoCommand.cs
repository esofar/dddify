using MyCompany.MyProject.Application.Todos.Queries;
using MyCompany.MyProject.Domain.Entities;
using MyCompany.MyProject.Domain.ValueObjects;
using MyCompany.MyProject.Infrastructure;

namespace MyCompany.MyProject.Application.Todos.Commands;

public class CreateTodoCommand : IRequest<TodoDto>
{
    public string Title { get; set; } = default!;

    public Colour Colour { get; set; } = default!;
}

public class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoDto>
{
    private readonly ApplicationDbContext _context;

    public CreateTodoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoDto> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = new Todo(request.Title, request.Colour);

        await _context.Todos.AddAsync(todo, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return todo.Adapt<TodoDto>();
    }
}
