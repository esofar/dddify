using Dddify.Domain;
using Dddify.Users;
using Dddify.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dddify.EntityFrameworkCore.Interceptors
{
    public class SaveEntityPropertiesInterceptor(IClock clock, ICurrentUser? currentUser = null) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyConcepts(eventData.Context);

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyConcepts(eventData.Context);

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

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
                        TryUpdateConcurrencyStamp(context, entry);
                        break;

                    case EntityState.Deleted:
                        TrySetDeletionProperties(entry);
                        break;
                }
            }
        }

        protected void TrySetCreationProperties(EntityEntry entry)
        {
            if (entry.Entity is ICreationAuditable entity)
            {
                entity.CreatedBy = currentUser?.Id;
                entity.CreatedAt = clock.Now;
            }
        }

        protected void TrySetModificationProperties(EntityEntry entry)
        {
            if (entry.Entity is IModificationAuditable entity)
            {
                entity.UpdatedBy = currentUser?.Id;
                entity.UpdatedAt = clock.Now;
            }
        }

        protected void TrySetDeletionProperties(EntityEntry entry)
        {
            if (entry.Entity is ISoftDeletable entity)
            {
                entry.Reload();
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;

                if (entry.Entity is IDeletionAuditable deletionAuditableEntity)
                {
                    deletionAuditableEntity.DeletedBy = currentUser?.Id;
                    deletionAuditableEntity.DeletedAt = clock.Now;
                }
            }
        }

        protected void TrySetConcurrencyStamp(EntityEntry entry)
        {
            if (entry.Entity is IHasConcurrencyStamp entity)
            {
                if (string.IsNullOrEmpty(entity.ConcurrencyStamp))
                {
                    entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
                }
            }
        }

        protected void TryUpdateConcurrencyStamp(DbContext? context, EntityEntry entry)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (entry.Entity is IHasConcurrencyStamp entity)
            {
                context.Entry(entity).Property(x => x.ConcurrencyStamp).OriginalValue = entity.ConcurrencyStamp;
                entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            }
        }
    }
}