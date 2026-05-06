using System.Linq.Expressions;

namespace Dddify.SharedKernel;

/// <summary>
/// Defines a repository for working with aggregate roots.
/// </summary>
/// <remarks>
/// Repositories provide collection-like access to aggregates while hiding persistence details from
/// the domain and application layers.
/// </remarks>
/// <typeparam name="TEntity">The aggregate root type.</typeparam>
/// <typeparam name="Tkey">The type of the aggregate root identifier.</typeparam>
public interface IRepository<TEntity, Tkey> where TEntity : Entity, IAggregateRoot
{
    /// <summary>
    /// Exposes the aggregate set as an <see cref="IQueryable{TEntity}"/>.
    /// </summary>
    /// <remarks>
    /// Prefer explicit, business-oriented repository methods whenever possible.
    /// Use this method only when a query cannot be expressed cleanly through a dedicated repository
    /// contract and the calling code understands the implications of deferred execution and query translation.
    /// </remarks>
    /// <returns>An <see cref="IQueryable{TEntity}"/> for the aggregate root type.</returns>
    public IQueryable<TEntity> AsQueryable();

    /// <summary>
    /// Gets an aggregate root by its identifier.
    /// </summary>
    /// <param name="id">The aggregate root identifier.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The matching aggregate root, or <see langword="null"/> if no match is found.</returns>
    Task<TEntity?> GetAsync(Tkey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all aggregate roots.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection containing all aggregate roots.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets aggregate roots that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate used to filter aggregate roots.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A collection of aggregate roots that satisfy the predicate.</returns>
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether any aggregate roots match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate used to filter aggregate roots.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns><see langword="true"/> if a matching aggregate root exists; otherwise, <see langword="false"/>.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new aggregate root to the repository.
    /// </summary>
    /// <param name="entity">The aggregate root to add.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a collection of aggregate roots to the repository.
    /// </summary>
    /// <param name="entities">The aggregate roots to add.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an aggregate root from the repository.
    /// </summary>
    /// <param name="entity">The aggregate root to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes a collection of aggregate roots from the repository.
    /// </summary>
    /// <param name="entities">The aggregate roots to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Marks an aggregate root as modified.
    /// </summary>
    /// <param name="entity">The aggregate root to update.</param>
    /// <remarks>
    /// This method typically delegates to the underlying persistence implementation and does not
    /// immediately commit changes. Persist the update through the current unit of work.
    /// </remarks>
    void Update(TEntity entity);

    /// <summary>
    /// Marks a collection of aggregate roots as modified.
    /// </summary>
    /// <param name="entities">The aggregate roots to update.</param>
    /// <remarks>
    /// This method typically delegates to the underlying persistence implementation and does not
    /// immediately commit changes. Persist the update through the current unit of work.
    /// </remarks>
    void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Applies an expected concurrency stamp to an aggregate root for optimistic concurrency checking.
    /// </summary>
    /// <param name="entity">The aggregate root to which the stamp should be applied.</param>
    /// <param name="concurrencyStamp">
    /// The expected concurrency stamp value.
    /// </param>
    /// <remarks>
    /// Call this before update or delete operations when the persistence implementation uses a
    /// concurrency token to detect conflicting writes.
    /// </remarks>
    void SetOriginalConcurrencyStamp(TEntity entity, string? concurrencyStamp);
}
