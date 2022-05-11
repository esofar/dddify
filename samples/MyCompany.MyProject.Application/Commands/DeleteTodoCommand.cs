using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using MyCompany.MyProject.Domain.ValueObjects;
using MyCompany.MyProject.Infrastructure;
using Dddify.Exceptions;
using MyCompany.MyProject.Domain.Entities;

namespace MyCompany.MyProject.Application.Commands;

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
        var todo = await _context.Todos.FindAsync(new object[] { request.Id }, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(nameof(Todo), request.Id);
        }

        _context.Todos.Remove(todo);

        await _context.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
