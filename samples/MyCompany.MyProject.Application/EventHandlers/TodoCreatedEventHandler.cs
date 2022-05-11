using Dddify.Domain.Events;
using MyCompany.MyProject.Domain.DomainEvents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MyCompany.MyProject.Application.DomainEventHandlers;

public class TodoCreatedEventHandler : IDomainEventHandler<TodoCreatedDomainEvent>
{
    public Task Handle(TodoCreatedDomainEvent notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
        //return Task.CompletedTask;
    }
}
