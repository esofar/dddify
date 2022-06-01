using Dddify.Auditing;
using Dddify.Domain.Entities;
using Dddify.Domain.Events;
using Dddify.Guids;
using Dddify.Infrastructure.EFCore;
using Dddify.Security.Identity;
using Dddify.Timing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dddify.Infrastructure.EntityFrameworkCore.Interceptors
{
    public class DefaultSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IClock _clock;
        private readonly IPublisher _publisher;
        private readonly ICurrentUser _currentUser;
        private readonly IGuidGenerator _guidGenerator;

        public DefaultSaveChangesInterceptor(IDbContextDependencies dependencies)
        {
            _clock = dependencies.Clock;
            _publisher = dependencies.Publisher;
            _currentUser = dependencies.CurrentUser;
            _guidGenerator = dependencies.GuidGenerator;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ArgumentNullException.ThrowIfNull(eventData.Context);

            ApplyConcepts(eventData.Context);

            foreach (var domainEvent in GetDispatchEvents(eventData.Context))
            {
                _publisher.Publish(domainEvent).GetAwaiter().GetResult();
            }

            return base.SavingChanges(eventData, result);
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(eventData.Context);

            ApplyConcepts(eventData.Context);

            foreach (var domainEvent in GetDispatchEvents(eventData.Context))
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        protected void ApplyConcepts(DbContext context)
        {
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
                        TryUpdateConcurrencyStamp(context, entry);
                        break;
                }
            }
        }

        protected IList<IDomainEvent> GetDispatchEvents(DbContext context)
        {
            return context.ChangeTracker
               .Entries<Entity>()
               .Where(c => c.Entity.DomainEvents != null && c.Entity.DomainEvents.Any())
               .SelectMany(x => x.Entity.DomainEvents)
               .ToList();
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
                entity.LastModifiedBy = _currentUser.Id;
                entity.LastModifiedAt = _clock.Now;
            }
        }

        protected void TrySetSoftDeletionProperties(EntityEntry entry)
        {
            if (entry.Entity is ISoftDeletable entity)
            {
                entry.Reload();
                entry.State = EntityState.Modified;
                entity.IsDeleted = true;

                TrySetModificationAuditProperties(entry);
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
