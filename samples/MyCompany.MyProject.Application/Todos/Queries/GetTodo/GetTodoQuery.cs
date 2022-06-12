using Dddify.Exceptions;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using MyCompany.MyProject.Domain.Entities;
using MyCompany.MyProject.Infrastructure;

namespace MyCompany.MyProject.Application.Todos.Queries;

public class GetTodoQuery : IRequest<TodoDto>
{
    public Guid Id { get; set; }
}

public class GetTodoQueryHandler : IRequestHandler<GetTodoQuery, TodoDto>
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetTodoQueryHandler(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<TodoDto> Handle(GetTodoQuery request, CancellationToken cancellationToken)
    {
        var todo = await _context.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (todo is null)
        {
            throw new NotFoundException(nameof(Todo), request.Id);
        }

        return todo.Adapt<TodoDto>();
    }
}
