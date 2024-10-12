using TodoListApp.Domain.Events;

namespace TodoListApp.Application.EventHandlers;

public class TodoItemCreatedEventHandler(ILogger<TodoItemCreatedEventHandler> logger)
    : IDomainEventHandler<TodoItemCreatedEvent>
{
    public async Task Handle(TodoItemCreatedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo item '{Text}' created.", @event.Text);

        await Task.CompletedTask;
    }
}