using Dddify.Domain.Events;
using MyCompany.MyProject.Domain.DomainEvents;

namespace MyCompany.MyProject.Application.Todos.EventHandlers;

public class TodoCreatedEventHandler : IDomainEventHandler<TodoCreatedDomainEvent>
{
    public Task Handle(TodoCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
