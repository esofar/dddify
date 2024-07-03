using Microsoft.EntityFrameworkCore;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Domain.Repositories;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
