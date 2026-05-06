using Dddify.SharedKernel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Dddify.EntityFrameworkCore.Interceptors;

/// <summary>
/// Dispatches domain events collected by aggregate roots before changes are persisted.
/// </summary>
/// <remarks>
/// This interceptor scans tracked aggregate roots, publishes their pending domain events through
/// MediatR, and clears the event collection to avoid duplicate dispatching.
/// </remarks>
public class DispatchDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    /// <summary>
    /// Dispatches pending domain events during synchronous save operations.
    /// </summary>
    /// <param name="eventData">The save-changes event data.</param>
    /// <param name="result">The current interception result.</param>
    /// <returns>The interception result to continue the save pipeline.</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var rows = base.SavingChanges(eventData, result);

        DispatchDomainEventsAsync(eventData.Context)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        return rows;
    }

    /// <summary>
    /// Dispatches pending domain events during asynchronous save operations.
    /// </summary>
    /// <param name="eventData">The save-changes event data.</param>
    /// <param name="result">The current interception result.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>The interception result to continue the save pipeline.</returns>
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var rows = await base.SavingChangesAsync(eventData, result, cancellationToken);

        await DispatchDomainEventsAsync(eventData.Context, cancellationToken);

        return rows;
    }

    /// <summary>
    /// Publishes all pending domain events from tracked aggregate roots in the current <see cref="DbContext"/>.
    /// </summary>
    /// <param name="context">The current EF Core database context.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous dispatch operation.</returns>
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
