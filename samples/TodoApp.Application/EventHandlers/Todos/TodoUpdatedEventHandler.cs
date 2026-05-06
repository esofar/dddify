using TodoApp.Domain.Events.Todos;

namespace TodoApp.Application.EventHandlers.Todos;

public class TodoUpdatedEventHandler(ILogger<TodoUpdatedEventHandler> logger) : IDomainEventHandler<TodoUpdatedDomainEvent>
{
    public async Task Handle(TodoUpdatedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo with id '{TodoId}' has been updated.", @event.Id);

        await Task.CompletedTask;
    }
}