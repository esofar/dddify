using Dddify.Exceptions;
using Microsoft.EntityFrameworkCore;
using MyCompany.MyProject.Application.Todos.Queries;
using MyCompany.MyProject.Domain.Entities;
using MyCompany.MyProject.Domain.ValueObjects;
using MyCompany.MyProject.Infrastructure;

namespace MyCompany.MyProject.Application.Todos.Commands;

public class UpdateTodoCommand : IRequest<TodoDto>
{
    public Guid Id { get; set; }

    public string Title { get; set; } = default!;

    public Colour Colour { get; set; } = default!;

    public string ConcurrencyStamp { get; set; } = default!;
}

public class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, TodoDto>
{
    private readonly ApplicationDbContext _context;

    public UpdateTodoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TodoDto> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(nameof(Todo), request.Id);
        }

        _context.ResetConcurrencyStamp(todo, request.ConcurrencyStamp);

        todo.Title = request.Title;
        todo.Colour = request.Colour;

        await _context.SaveChangesAsync(cancellationToken);

        return todo.Adapt<TodoDto>();
    }
}
