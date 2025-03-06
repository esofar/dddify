
namespace TodoListApp.Application.Commands;

public record DeleteTodoCommand(Guid Id) : ICommand;

public class DeleteTodoCommandValidator : AbstractValidator<DeleteTodoCommand>
{
    public DeleteTodoCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteTodoCommandHandler(ITodoRepository todoRepository, IPublisher publisher) : ICommandHandler<DeleteTodoCommand>
{
    public async Task Handle(DeleteTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new TodoNotFoundException(command.Id);

        todoRepository.Remove(todo);

        await publisher.Publish(new TodoDeletedDomainEvent(todo.Id));
    }
}