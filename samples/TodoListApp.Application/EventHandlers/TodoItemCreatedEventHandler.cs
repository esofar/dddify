using Dddify.Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using TodoListApp.Application.Commands;
using TodoListApp.Domain.DomainEvents;
using TodoListApp.Domain.Repositories;
using TodoListApp.Domain.Exceptions;

namespace TodoListApp.Application.EventHandlers;

public class TodoItemCreatedEventHandler(
    ISender sender,
    ITodoItemRepository todoItemRepository,
    IUnitOfWork unitOfWork,
    ILogger<TodoItemCreatedEventHandler> logger) : IDomainEventHandler<TodoItemCreatedDomainEvent>
{
    public async Task Handle(TodoItemCreatedDomainEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Todo item '{Text}' created.", @event.TodoItem.Text);

        //await sender.Send(new DeleteTodoItemCommand(new Guid("E03CD2DE-8CEA-4FBF-A24E-87CF3A5B242E")), cancellationToken);

        var id = new Guid("B7EC9FEF-C12D-4216-89EA-06BAD2CD7477");

        var other = await todoItemRepository.GetAsync(id, cancellationToken)
            ?? throw new TodoItemNotFoundException(id);

        await todoItemRepository.RemoveAsync(other);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
