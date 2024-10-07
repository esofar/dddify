using Microsoft.EntityFrameworkCore.Storage;

namespace Dddify.Domain;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default);

    IDbContextTransaction? CurrentTransaction { get; }

    IDbContextTransaction BeginTransaction();

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);
}