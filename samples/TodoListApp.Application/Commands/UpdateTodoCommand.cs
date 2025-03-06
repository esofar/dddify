using TodoListApp.Domain.Exceptions;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Commands;

public record UpdateTodoCommand(Guid Id, bool IsDone) : ICommand;

public class UpdateTodoCommandValidator : AbstractValidator<UpdateTodoCommand>
{
    public UpdateTodoCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class UpdateTodoCommandHandler(ITodoRepository todoRepository) : ICommandHandler<UpdateTodoCommand>
{
    public async Task Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.Id, cancellationToken)
            ?? throw new TodoNotFoundException(command.Id);

        todo.Update(command.IsDone);
    }
}
