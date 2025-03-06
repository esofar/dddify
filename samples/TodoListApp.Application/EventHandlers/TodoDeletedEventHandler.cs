namespace TodoListApp.Application.EventHandlers;

public class TodoDeletedEventHandler(ILogger<TodoDeletedEventHandler> logger) : IDomainEventHandler<TodoDeletedDomainEvent>
{
    public async Task Handle(TodoDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo with id '{TodoId}' has been deleted.", @event.TodoId);

        await Task.CompletedTask;
    }
}