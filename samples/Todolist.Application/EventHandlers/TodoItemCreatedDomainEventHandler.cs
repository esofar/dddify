namespace Todolist.Application.EventHandlers;

public class TodoListItemCreatedDomainEventHandler : IDomainEventHandler<TodoListItemCreatedDomainEvent>
{
    public Task Handle(TodoListItemCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
