namespace TodoListApp.Application.EventHandlers;

public class TodoUpdatedEventHandler(ILogger<TodoUpdatedEventHandler> logger) : IDomainEventHandler<TodoUpdatedDomainEvent>
{
    public async Task Handle(TodoUpdatedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo with id '{TodoId}' has been updated. IsDone: '{IsDone}'.", @event.TodoId, @event.IsDone);

        await Task.CompletedTask;
    }
}