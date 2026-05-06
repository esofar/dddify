using TodoApp.Domain.Events.Todos;

namespace TodoApp.Application.EventHandlers.Todos;

public class TodoCreatedEventHandler(ILogger<TodoCreatedEventHandler> logger) : IDomainEventHandler<TodoCreatedDomainEvent>
{
    public async Task Handle(TodoCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo with id '{Id}' created.", @event.Id);

        await Task.CompletedTask;
    }
}