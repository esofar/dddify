using Dddify.Exceptions;
using MediatR;
using MyCompany.MyProject.Domain.Entities;
using MyCompany.MyProject.Infrastructure;
using Dddify.Domain.Events;
using MyCompany.MyProject.Domain.DomainEvents;
using Microsoft.EntityFrameworkCore;

namespace MyCompany.MyProject.Application.Todos.Commands;

public class DeleteTodoCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand>
{
    private readonly ApplicationDbContext _context;

    public DeleteTodoCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(nameof(Todo), request.Id);
        }

        _context.Todos.Remove(todo);

        todo.AddDomainEvent(new TodoDeletedDomainEvent(todo));

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
