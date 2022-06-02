using Dddify.Domain.Events;
using MyCompany.MyProject.Domain.Entities;

namespace MyCompany.MyProject.Domain.DomainEvents;

public class TodoDeletedDomainEvent : IDomainEvent
{
    public Todo Todo { get; private set; }

    public TodoDeletedDomainEvent(Todo todo)
    {
        Todo = todo;
    }
}
