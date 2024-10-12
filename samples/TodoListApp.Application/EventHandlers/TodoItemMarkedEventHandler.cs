using TodoListApp.Domain.Events;

namespace TodoListApp.Application.EventHandlers;

public class TodoItemMarkedEventHandler(ILogger<TodoItemMarkedEventHandler> logger) 
    : IDomainEventHandler<TodoItemMarkedEvent>
{
    public async Task Handle(TodoItemMarkedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo item with ID '{TodoItemId}' marked as '{IsDone}'.", @event.TodoItemId, @event.IsDone);

        await Task.CompletedTask;
    }
}