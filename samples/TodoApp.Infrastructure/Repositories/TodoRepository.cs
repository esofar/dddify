using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Infrastructure.Repositories;

public sealed class TodoRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, Todo, Guid>(context), ITodoRepository
{
}