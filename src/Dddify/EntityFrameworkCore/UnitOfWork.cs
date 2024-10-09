using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.EntityFrameworkCore;

public class UnitOfWork<TDbContext>(TDbContext context) : IUnitOfWork
    where TDbContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;

    public IDbContextTransaction? CurrentTransaction => _currentTransaction;

    public IDbContextTransaction BeginTransaction()
    {
        _currentTransaction = context.Database.BeginTransaction();
        return _currentTransaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.CommitAsync(cancellationToken);
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            await _currentTransaction.RollbackAsync(cancellationToken);
            _currentTransaction.Dispose();
            _currentTransaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = BeginTransaction();

            await using (_currentTransaction)
            {
                try
                {
                    var rows = await context.SaveChangesAsync(cancellationToken);

                    await CommitAsync(cancellationToken);

                    return rows;
                }
                catch
                {
                    await RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }
        else
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
    }
}