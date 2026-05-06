using Dddify.SharedKernel;
using Dddify.SharedKernel.Auditing;
using Dddify.Timing;
using Dddify.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace Dddify.EntityFrameworkCore.Interceptors;

/// <summary>
/// Applies Dddify entity-state conventions before changes are persisted.
/// </summary>
/// <remarks>
/// This interceptor populates auditing fields, converts deletes into soft deletes when supported,
/// and refreshes concurrency stamps for modified aggregates and entities.
/// </remarks>
public class ApplyEntityStateInterceptor(
    IClock? clock = null,
    ICurrentUser? currentUser = null,
    IOptions<TimingOptions>? timingOptions = null) : SaveChangesInterceptor
{
    /// <summary>
    /// Applies entity-state conventions during synchronous save operations.
    /// </summary>
    /// <param name="eventData">The save-changes event data.</param>
    /// <param name="result">The current interception result.</param>
    /// <returns>The interception result to continue the save pipeline.</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        ApplyConcepts(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Applies entity-state conventions during asynchronous save operations.
    /// </summary>
    /// <param name="eventData">The save-changes event data.</param>
    /// <param name="result">The current interception result.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The interception result to continue the save pipeline.</returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        ApplyConcepts(eventData.Context);

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Applies auditing, soft-delete, and concurrency rules to tracked entities.
    /// </summary>
    /// <param name="context">The current EF Core database context.</param>
    protected void ApplyConcepts(DbContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        foreach (var entry in context.ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    TrySetCreationProperties(entry);
                    TrySetConcurrencyStamp(entry);
                    break;

                case EntityState.Modified:
                    TrySetModificationProperties(entry);
                    TrySetConcurrencyStamp(entry);
                    break;

                case EntityState.Deleted:
                    TrySetDeletionProperties(entry);
                    break;
            }
        }
    }

    /// <summary>
    /// Sets creation auditing properties for a newly added entity.
    /// </summary>
    /// <param name="entry">The tracked entity entry.</param>
    protected void TrySetCreationProperties(EntityEntry entry)
    {
        if (entry.Entity is IHasCreatedBy hasCreatedBy)
        {
            hasCreatedBy.CreatedBy = GetAuditUserId();
        }

        if (entry.Entity is IHasCreatedAt hasCreatedAt)
        {
            hasCreatedAt.CreatedAt = GetAuditTimestamp();
        }
    }

    /// <summary>
    /// Sets modification auditing properties for a modified entity.
    /// </summary>
    /// <param name="entry">The tracked entity entry.</param>
    protected void TrySetModificationProperties(EntityEntry entry)
    {
        if (entry.Entity is IHasModifiedBy hasModifiedBy)
        {
            hasModifiedBy.ModifiedBy = GetAuditUserId();
        }

        if (entry.Entity is IHasModifiedAt hasModifiedAt)
        {
            hasModifiedAt.ModifiedAt = GetAuditTimestamp();
        }
    }

    /// <summary>
    /// Converts a delete operation into a soft delete when the entity supports soft deletion.
    /// </summary>
    /// <param name="entry">The tracked entity entry.</param>
    protected void TrySetDeletionProperties(EntityEntry entry)
    {
        if (entry.Entity is ISoftDeletable entity)
        {
            entry.Reload();

            entry.State = EntityState.Modified;
            entity.IsDeleted = true;

            if (entry.Entity is IHasDeletedBy hasDeletedBy)
            {
                hasDeletedBy.DeletedBy = GetAuditUserId();
            }

            if (entry.Entity is IHasDeletedAt hasDeletedAt)
            {
                hasDeletedAt.DeletedAt = GetAuditTimestamp();
            }
        }
    }

    /// <summary>
    /// Refreshes the concurrency stamp when the entity supports optimistic concurrency.
    /// </summary>
    /// <param name="entry">The tracked entity entry.</param>
    protected static void TrySetConcurrencyStamp(EntityEntry entry)
    {
        if (entry.Entity is IHasConcurrencyStamp entity)
        {
            entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }
    }

    /// <summary>
    /// Gets the configured audit timestamp value.
    /// </summary>
    private DateTimeOffset? GetAuditTimestamp()
        => (timingOptions?.Value.AuditTimeSource ?? AuditTimeSource.UtcNow) switch
        {
            AuditTimeSource.Now => clock?.Now,
            AuditTimeSource.UtcNow => clock?.UtcNow,
            _ => throw new NotSupportedException("Unsupported audit time source.")
        };

    /// <summary>
    /// Gets the configured audit user ID.
    /// </summary>
    private string? GetAuditUserId()
        => currentUser?.Id;
}
