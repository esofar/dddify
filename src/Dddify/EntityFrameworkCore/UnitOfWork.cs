using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Provides the default EF Core implementation of <see cref="IUnitOfWork"/>.
/// </summary>
/// <typeparam name="TDbContext">The <see cref="DbContext"/> type used to persist changes.</typeparam>
public class UnitOfWork<TDbContext>(TDbContext context) : IUnitOfWork where TDbContext : DbContext
{
    /// <summary>
    /// Persists all tracked changes through the underlying <see cref="DbContext"/>.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    private IDbContextTransaction? _transaction;

    /// <summary>
    /// Gets the current active database transaction, if one exists.
    /// </summary>
    public IDbContextTransaction? CurrentTransaction => _transaction;

    /// <summary>
    /// Begins a new database transaction on the underlying <see cref="DbContext"/>.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The started transaction.</returns>
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            throw new InvalidOperationException("A transaction is already active for the current unit of work.");
        }

        return _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    /// <summary>
    /// Commits the current transaction and disposes it.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_transaction);

        var transaction = _transaction;

        try
        {
            await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            await transaction.DisposeAsync();
            _transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction and disposes it.
    /// </summary>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_transaction);

        var transaction = _transaction;

        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await transaction.DisposeAsync();
            _transaction = null;
        }
    }
}
