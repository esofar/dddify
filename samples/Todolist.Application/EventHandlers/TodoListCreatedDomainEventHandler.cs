namespace Todolist.Application.EventHandlers;

public class TodoListCreatedDomainEventHandler : IDomainEventHandler<TodoListCreatedDomainEvent>
{
    public Task Handle(TodoListCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
