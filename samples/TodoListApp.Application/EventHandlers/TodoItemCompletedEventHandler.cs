using Dddify.Domain;
using Microsoft.Extensions.Logging;
using TodoListApp.Domain.Events;

namespace TodoListApp.Application.EventHandlers;

public class TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger) : IDomainEventHandler<TodoItemCompletedEvent>
{
    public async Task Handle(TodoItemCompletedEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo item with ID '{TodoItemId}' completed to '{IsDone}'.", @event.TodoItem.Id, @event.TodoItem.IsDone);

        await Task.CompletedTask;
    }
}
