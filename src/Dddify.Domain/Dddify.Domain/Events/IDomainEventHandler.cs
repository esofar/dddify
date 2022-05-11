using MediatR;

namespace Dddify.Domain.Events;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{

}
