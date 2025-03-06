using Dddify.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dddify.EntityFrameworkCore.Interceptors
{
    public class DispatchDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            var rows = base.SavingChanges(eventData, result);

            DispatchDomainEventsAsync(eventData.Context)
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            return rows;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var rows = await base.SavingChangesAsync(eventData, result, cancellationToken);

            await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

            return rows;
        }

        protected async Task DispatchDomainEventsAsync(DbContext? context, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(context);

            var entries = context.ChangeTracker
                .Entries<IAggregateRoot>()
                .Where(entry => entry.Entity.DomainEvents.Count != 0)
                .ToList();

            var domainEvents = entries
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();

            entries.ForEach(entry => entry.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents)
            {
                await publisher.Publish(domainEvent, cancellationToken);
            }
        }
    }
}