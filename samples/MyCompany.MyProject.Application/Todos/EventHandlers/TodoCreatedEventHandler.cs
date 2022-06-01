using Dddify.Domain.Events;
using MyCompany.MyProject.Domain.DomainEvents;
using System.Threading;
using System.Threading.Tasks;

namespace MyCompany.MyProject.Application.Todos.EventHandlers;

public class TodoCreatedEventHandler : IDomainEventHandler<TodoCreatedDomainEvent>
{
    public Task Handle(TodoCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
