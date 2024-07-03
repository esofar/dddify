using Dddify.Domain;
using Dddify.Domain.Entities;
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
        private readonly IClock _clock = clock;
        private readonly IPublisher _publisher = publisher;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly IGuidGenerator _guidGenerator = guidGenerator;

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ApplyConcepts(eventData.Context);
            DispatchDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyConcepts(eventData.Context);
            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

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
                .Entries<IAggregateRoot>()
                .Where(c => c.Entity.DomainEvents.Count != 0)
                .ToList();

            var domainEvents = entities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            entities.ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
        }

        protected void TrySetCreationAuditProperties(EntityEntry entry)
        {
            if (entry.Entity is ICreationAudited entity)
            {
                entity.CreatedBy = _currentUser.Id;
                entity.CreatedAt = _clock.Now;
            }
        }

        protected void TrySetModificationAuditProperties(EntityEntry entry)
        {
            if (entry.Entity is IModificationAudited entity)
            {
                entity.UpdatedBy = _currentUser.Id;
                entity.UpdatedAt = _clock.Now;
            }
        }

        protected void TrySetSoftDeletionProperties(EntityEntry entry)
        {
            if (entry.Entity is ISoftDeletable entity)
            {
                entry.Reload();

                entry.State = EntityState.Modified;
                entity.IsDeleted = true;
                entity.DeletedBy = _currentUser.Id;
                entity.DeletedAt = _clock.Now;
            }
        }

        protected void TrySetConcurrencyStampIfNull(EntityEntry entry)
        {
            if (entry.Entity is IHasConcurrencyStamp entity)
            {
                if (string.IsNullOrEmpty(entity.ConcurrencyStamp))
                {
                    entity.ConcurrencyStamp = _guidGenerator.CreateAsString();
                }
            }
        }

        protected void TryUpdateConcurrencyStamp(DbContext? context, EntityEntry entry)
        {
            ArgumentNullException.ThrowIfNull(context);

            if (entry.Entity is IHasConcurrencyStamp entity)
            {
                context.Entry(entity).Property(x => x.ConcurrencyStamp).OriginalValue = entity.ConcurrencyStamp;
                entity.ConcurrencyStamp = _guidGenerator.CreateAsString();
            }
        }

        protected void TrySetGuidId(EntityEntry entry)
        {
            if (entry.Entity is IEntity<Guid> entity)
            {
                if (entity.Id != default)
                {
                    return;
                }

                entity.Id = _guidGenerator.Create();
            }
        }
    }
}