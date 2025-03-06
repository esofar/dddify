using TodoListApp.Infrastructure.Data;

namespace TodoListApp.Infrastructure.Repositories;

public class TodoRepository(AppDbContext context) : RepositoryBase<AppDbContext, Todo, Guid>(context), ITodoRepository
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<Todo>> GetSortedTodosAsync()
    {
        return await _context.Todos
            .AsNoTracking()
            .OrderBy(c => c.IsDone)
            .ThenByDescending(c => c.Priority)
            .ThenBy(c => c.CreatedAt)
            .ToListAsync();
    }
}
