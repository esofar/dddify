using Dddify.Domain;
using Microsoft.Extensions.Logging;
using TodoListApp.Domain.DomainEvents;

namespace TodoListApp.Application.EventHandlers;

public class TodoItemCompletedEventHandler(ILogger<TodoItemCompletedEventHandler> logger) : IDomainEventHandler<TodoItemCompletedDomainEvent>
{
    public async Task Handle(TodoItemCompletedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo item with ID '{TodoItemId}' completed to '{IsDone}'.", @event.TodoItem.Id, @event.TodoItem.IsDone);

        await Task.CompletedTask;
    }
}