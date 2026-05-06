using Dddify.SharedKernel;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Provides a base EF Core repository implementation for aggregate roots.
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type used for persistence.</typeparam>
/// <typeparam name="TEntity">The aggregate root type.</typeparam>
/// <typeparam name="Tkey">The aggregate root identifier type.</typeparam>
public abstract class RepositoryBase<TDbContext, TEntity, Tkey>(TDbContext context) : IRepository<TEntity, Tkey>
    where TDbContext : DbContext
    where TEntity : AggregateRoot<Tkey>
{
    /// <summary>
    /// Exposes the aggregate set as a queryable source.
    /// </summary>
    /// <returns>An <see cref="IQueryable{T}"/> for the aggregate root set.</returns>
    public IQueryable<TEntity> AsQueryable() => context.Set<TEntity>();

    /// <summary>
    /// Gets all aggregate roots.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection containing all aggregate roots.</returns>
    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().ToListAsync(cancellationToken);

    /// <summary>
    /// Gets an aggregate root by its identifier.
    /// </summary>
    /// <param name="id">The aggregate root identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The matching aggregate root, or <see langword="null"/> if no match is found.</returns>
    public async virtual Task<TEntity?> GetAsync(Tkey id, CancellationToken cancellationToken = default)
        => await context.FindAsync<TEntity>([id], cancellationToken);

    /// <summary>
    /// Gets aggregate roots that satisfy the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection of matching aggregate roots.</returns>
    public async virtual Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().Where(predicate).ToListAsync(cancellationToken);

    /// <summary>
    /// Determines whether any aggregate root satisfies the specified predicate.
    /// </summary>
    /// <param name="predicate">The filter predicate.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns><see langword="true"/> if a matching aggregate root exists; otherwise, <see langword="false"/>.</returns>
    public async virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await context.Set<TEntity>().AnyAsync(predicate, cancellationToken);

    /// <summary>
    /// Adds a new aggregate root to the current persistence context.
    /// </summary>
    /// <param name="entity">The aggregate root to add.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    public async virtual Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await context.AddAsync(entity, cancellationToken);

    /// <summary>
    /// Adds a collection of aggregate roots to the current persistence context.
    /// </summary>
    /// <param name="entities">The aggregate roots to add.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    public async virtual Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await context.AddRangeAsync(entities, cancellationToken);

    /// <summary>
    /// Marks an aggregate root for removal.
    /// </summary>
    /// <param name="entity">The aggregate root to remove.</param>
    public virtual void Remove(TEntity entity)
       => context.Remove(entity);

    /// <summary>
    /// Marks a collection of aggregate roots for removal.
    /// </summary>
    /// <param name="entities">The aggregate roots to remove.</param>
    public virtual void RemoveRange(IEnumerable<TEntity> entities)
       => context.RemoveRange(entities);

    /// <summary>
    /// Marks an aggregate root as modified.
    /// </summary>
    /// <param name="entity">The aggregate root to update.</param>
    public virtual void Update(TEntity entity)
       => context.Update(entity);

    /// <summary>
    /// Marks a collection of aggregate roots as modified.
    /// </summary>
    /// <param name="entities">The aggregate roots to update.</param>
    public virtual void UpdateRange(IEnumerable<TEntity> entities)
       => context.UpdateRange(entities);

    /// <summary>
    /// Sets the original concurrency stamp value used by EF Core for optimistic concurrency checks.
    /// </summary>
    /// <param name="entity">The aggregate root whose original concurrency stamp should be set.</param>
    /// <param name="concurrencyStamp">The expected original concurrency stamp value.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entity"/> is <see langword="null"/>.</exception>
    public virtual void SetOriginalConcurrencyStamp(TEntity entity, string? concurrencyStamp)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity is not IHasConcurrencyStamp)
        {
            return;
        }

        context.Entry(entity)
            .Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp))
            .OriginalValue = concurrencyStamp;
    }
}
