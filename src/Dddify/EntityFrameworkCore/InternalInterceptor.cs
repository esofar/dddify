using Dddify.Domain;
using Dddify.Guids;
using Dddify.Identity;
using Dddify.Timing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dddify.EntityFrameworkCore
{
    public class InternalInterceptor(
        IClock clock,
        IPublisher publisher,
        ICurrentUser currentUser,
        IGuidGenerator guidGenerator) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyConcepts(eventData.Context);

            var rows = base.SavingChanges(eventData, result);

            DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();

            return rows;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyConcepts(eventData.Context);

            var rows = await base.SavingChangesAsync(eventData, result, cancellationToken);

            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

            return rows;
        }

        protected void ApplyConcepts(DbContext? context)
        {
            ArgumentNullException.ThrowIfNull(context);

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        TrySetGuidId(entry);
                        TrySetCreationAuditProperties(entry);
                        TrySetConcurrencyStampIfNull(entry);
                        break;

                    case EntityState.Modified:
                        TrySetModificationAuditProperties(entry);
                        TryUpdateConcurrencyStamp(context, entry);
                        break;

                    case EntityState.Deleted:
                        TrySetSoftDeletionProperties(entry);
                        break;
                }
            }
        }

        protected async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            var entities = context.ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(c => c.Entity.DomainEvents.Count != 0)
                .ToList();

            var domainEvents = entities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            entities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await publisher.Publish(domainEvent, cancellationToken);
            }
        }

        protected void TrySetCreationAuditProperties(EntityEntry entry)
        {
            if (entry.Entity is ICreationAuditable entity)
            {
                entity.CreatedBy = currentUser.Id;
                entity.CreatedAt = clock.Now;
            }
        }

        protected void TrySetModificationAuditProperties(EntityEntry entry)
        {
            if (entry.Entity is IModificationAuditable entity)
            {
                entity.UpdatedBy = currentUser.Id;
                entity.UpdatedAt = clock.Now;
            }
        }

        protected void TrySetSoftDeletionProperties(EntityEntry entry)
        {
            if (entry.Entity is ISoftDeletable entity)
            {
                entry.Reload();
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
                entity.DeletedBy = currentUser.Id;
                entity.DeletedAt = clock.Now;
            }
        }

        protected void TrySetConcurrencyStampIfNull(EntityEntry entry)
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

        protected void TrySetGuidId(EntityEntry entry)
        {
            if (entry.Entity is IEntity<Guid> entity && entity.Id == default)
            {
                entity.Id = guidGenerator.Create();
            }
        }
    }
}