using System.Linq.Expressions;

namespace Dddify.Domain;

/// <summary>
/// Represents a generic repository that provides basic CRUD operations.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="Tkey">The type of the key.</typeparam>
public interface IRepository<TEntity, Tkey> where TEntity : Entity, IAggregateRoot
{
    /// <summary>
    /// Returns an <see cref="IQueryable{TEntity}"/> for the entity.
    /// </summary>
    /// <remarks>
    /// Prioritize defining explicit, business-meaningful methods in the repository interface (e.g., `GetById`, `FindByCondition`, `ListAll`) to fulfill data access needs.
    /// Only consider using <see cref="AsQueryable"/> when truly dynamic, complex queries are required, and the repository interface cannot directly support them.
    /// In such cases, carefully evaluate the potential risks and ensure a thorough understanding of the underlying data access logic and performance implications.
    /// </remarks>
    /// <returns>An <see cref="IQueryable{TEntity}"/> instance for the entity.</returns>
    public IQueryable<TEntity> AsQueryable();

    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    Task<TEntity?> GetAsync(Tkey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of all entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a list of entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A collection of entities that match the predicate.</returns>
    Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether any entities match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>True if any entities match the predicate; otherwise, false.</returns>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a range of new entities.
    /// </summary>
    /// <param name="entities">The entities to add.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes a range of entities.
    /// </summary>
    /// <param name="entities">The entities to remove.</param>
    void RemoveRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    void Update(TEntity entity);

    /// <summary>
    /// Updates a range of entities.
    /// </summary>
    /// <param name="entities">The entities to update.</param>
    void UpdateRange(IEnumerable<TEntity> entities);
}
