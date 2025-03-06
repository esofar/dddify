using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

public abstract class RepositoryBase<TDbContext, TEntity, Tkey>(TDbContext context) : IRepository<TEntity, Tkey>
    where TDbContext : DbContext
    where TEntity : Entity, IAggregateRoot
    where Tkey : IComparable<Tkey>
{
    public IQueryable<TEntity> AsQueryable() => context.Set<TEntity>();

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().ToListAsync(cancellationToken);

    public async virtual Task<TEntity?> GetAsync(Tkey id, CancellationToken cancellationToken = default)
        => await context.FindAsync<TEntity>([id], cancellationToken);

    public async virtual Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

    public async virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().AnyAsync(predicate, cancellationToken);

    public async virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await context.AddAsync(entity, cancellationToken);

    public async virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await context.AddRangeAsync(entities, cancellationToken);

    public virtual void Remove(TEntity entity)
       => context.Remove(entity);

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
       => context.RemoveRange(entities);

    public virtual void Update(TEntity entity)
       => context.Update(entity);

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
       => context.UpdateRange(entities);
}
