using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.EntityFrameworkCore;

public class UnitOfWork<TDbContext>(TDbContext context) : IUnitOfWork where TDbContext : DbContext
{
    private IDbContextTransaction? _transaction;

    public bool HasActiveTransaction => _transaction is not null;

    public async Task<IDbContextTransaction> BeginAsync(CancellationToken cancellationToken = default)
    {
        return _transaction = await context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_transaction);

        await _transaction.CommitAsync(cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(_transaction);

        await _transaction.RollbackAsync(cancellationToken);
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (HasActiveTransaction)
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            _transaction = await BeginAsync(cancellationToken);

            await using (_transaction)
            {
                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                    await CommitAsync(cancellationToken);
                }
                catch
                {
                    await RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
    }
}