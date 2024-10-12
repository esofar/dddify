using TodoListApp.Domain.Entities;

namespace TodoListApp.Application.Contracts;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    //Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
