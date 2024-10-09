using System.Linq.Expressions;

namespace Dddify.Domain;

public interface IRepository<TEntity> where TEntity : Entity, IAggregateRoot
{
    public IQueryable<TEntity> Queryable();

    public IQueryable<TEntity> AsNoTrackingQueryable();

    Task<TEntity?> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    Task RemoveAsync(TEntity entity);

    Task RemoveRangeAsync(IEnumerable<TEntity> entities);

    Task UpdateAsync(TEntity entity);

    Task UpdateRangeAsync(IEnumerable<TEntity> entities);
}
