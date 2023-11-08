using Dddify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Todolist.Domain.Entities;

namespace Todolist.Domain.Repositories;

public interface IApplicationDbContext
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    void ResetConcurrencyStamp<TEntity>(TEntity entity, string concurrencyStamp)
        where TEntity : IEntity;

    DbSet<TodoList> TodoLists { get; }
}
