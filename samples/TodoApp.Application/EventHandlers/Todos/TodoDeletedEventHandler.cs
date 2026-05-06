using TodoApp.Domain.Events.Todos;

namespace TodoApp.Application.EventHandlers.Todos;

public class TodoDeletedEventHandler(ILogger<TodoDeletedEventHandler> logger) : IDomainEventHandler<TodoDeletedDomainEvent>
{
    public async Task Handle(TodoDeletedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo with id '{TodoId}' has been deleted.", @event.Id);

        await Task.CompletedTask;
    }
}