using Dddify.Domain.Events;
using MyCompany.MyProject.Domain.DomainEvents;
using System.Threading;
using System.Threading.Tasks;

namespace MyCompany.MyProject.Application.Todos.EventHandlers;

public class TodoDeletedEventHandler : IDomainEventHandler<TodoDeletedDomainEvent>
{
    public Task Handle(TodoDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
