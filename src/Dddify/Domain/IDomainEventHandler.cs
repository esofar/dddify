using MediatR;

namespace Dddify.Domain;

/// <summary>
/// Defines a handler for a domain event.
/// </summary>
/// <typeparam name="TDomainEvent">The type of domain event.</typeparam>
public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event asynchronously.
    /// </summary>
    /// <param name="event">The domain event to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    new Task Handle(TDomainEvent @event, CancellationToken cancellationToken);
}