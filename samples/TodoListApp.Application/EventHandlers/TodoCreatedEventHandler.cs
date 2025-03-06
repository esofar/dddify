namespace TodoListApp.Application.EventHandlers;

public class TodoCreatedEventHandler(ILogger<TodoCreatedEventHandler> logger) : IDomainEventHandler<TodoCreatedDomainEvent>
{
    public async Task Handle(TodoCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo with id '{TodoId}' [{Text}] created.", @event.TodoId, @event.Text);

        await Task.CompletedTask;
    }
}