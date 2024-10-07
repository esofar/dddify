using Dddify.Domain;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.EntityFrameworkCore;

public class UnitOfWork<TDbContext>(TDbContext context) : IUnitOfWork
    where TDbContext : AppDbContext
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

    public async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction == null)
        {
            _currentTransaction = BeginTransaction();

            await using (_currentTransaction)
            {
                try
                {
                    var rows = await context.SaveChangesAsync(cancellationToken);
                    // TODO: await _mediator.DispatchDomainEventsAsync(this, 0, cancellationToken);
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
            var rows = await context.SaveChangesAsync(cancellationToken);
            // TODO: await _mediator.DispatchDomainEventsAsync(this, 0, cancellationToken);
            return rows;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

}