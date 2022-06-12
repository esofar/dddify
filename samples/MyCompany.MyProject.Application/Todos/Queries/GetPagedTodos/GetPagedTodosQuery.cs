using Dddify.Infrastructure.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyCompany.MyProject.Infrastructure;

namespace MyCompany.MyProject.Application.Todos.Queries;

public class GetPagedTodosQuery : IRequest<IPagedList<TodoDto>>
{
    public int Page { get; set; }
    public int Size { get; set; }
}

public class GetTodoPagedListQueryHandler : IRequestHandler<GetPagedTodosQuery, IPagedList<TodoDto>>
{
    private readonly ApplicationDbContext _context;

    public GetTodoPagedListQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IPagedList<TodoDto>> Handle(GetPagedTodosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Todos
            .AsNoTracking()
            .ProjectToType<TodoDto>()
            .OrderBy(c => c.Title)
            .ToPagedListAsync(request.Page, request.Size, cancellationToken);
    }
}
