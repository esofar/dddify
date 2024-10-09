using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

public abstract class Repository<TDbContext, TEntity>(TDbContext context) : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : Entity, IAggregateRoot
{
    public IQueryable<TEntity> Queryable() => context.Set<TEntity>();

    public IQueryable<TEntity> AsNoTrackingQueryable() => context.Set<TEntity>().AsNoTracking();

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>().ToListAsync(cancellationToken);
    }

    public async virtual Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.FindAsync<TEntity>([id], cancellationToken);
    }

    public async virtual Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);
    }

    public async virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await context.Set<TEntity>().AnyAsync(predicate, cancellationToken);
    }

    public async virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await context.AddAsync(entity, cancellationToken);
    }

    public async virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await context.AddRangeAsync(entities, cancellationToken);
    }

    public async virtual Task RemoveAsync(TEntity entity)
    {
        await Task.FromResult(context.Remove(entity));
    }

    public async virtual Task RemoveRangeAsync(IEnumerable<TEntity> entities)
    {
        context.RemoveRange(entities);
        await Task.CompletedTask;
    }

    public async virtual Task UpdateAsync(TEntity entity)
    {
        await Task.FromResult(context.Update(entity));
    }

    public async virtual Task UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        context.UpdateRange(entities);
        await Task.CompletedTask;
    }
}
