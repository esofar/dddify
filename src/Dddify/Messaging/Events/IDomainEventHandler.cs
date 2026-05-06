using Dddify.SharedKernel;
using MediatR;

namespace Dddify.Messaging.Events;

/// <summary>
/// Defines a handler for a domain event.
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event to handle.</typeparam>
public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handles a domain event.
    /// </summary>
    /// <param name="event">The domain event instance.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous handling operation.</returns>
    new Task Handle(TDomainEvent @event, CancellationToken cancellationToken);
}
