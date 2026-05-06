using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Defines the contract for coordinating persistence operations within a single business transaction.
/// </summary>
/// <remarks>
/// A unit of work groups repository operations, controls the underlying database transaction,
/// and persists changes as one logical commit.
/// </remarks>
public interface IUnitOfWork
{
    /// <summary>
    /// Gets the current active database transaction, if one has been started.
    /// </summary>
    IDbContextTransaction? CurrentTransaction { get; }

    /// <summary>
    /// Persists all pending changes tracked by the current unit of work.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// A task whose result is the number of state entries written to the underlying store.
    /// </returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction for the current unit of work.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>
    /// A task whose result is the started transaction.
    /// </returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous commit operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous rollback operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
