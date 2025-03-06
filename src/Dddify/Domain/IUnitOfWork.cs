using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.Domain;

/// <summary>
/// Defines the contract for a unit of work that manages transactions and changes to the database.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Indicates if there is an active transaction.
    /// </summary>
    bool HasActiveTransaction { get; }

    /// <summary>
    /// Saves changes made within the unit of work to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token for canceling the operation.</param>
    /// <returns>The number of affected rows.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <returns>The current database transaction instance.</returns>
    Task<IDbContextTransaction> BeginAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the changes of the current transaction to the database asynchronously.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token for canceling the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the changes of the current transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">An optional cancellation token for canceling the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}