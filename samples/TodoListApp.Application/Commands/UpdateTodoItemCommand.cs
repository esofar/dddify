using Dddify.Messaging.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Domain.Exceptions;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Commands;

public record UpdateTodoItemCommand(Guid Id, bool IsDone) : ICommand;

public class UpdateTodoItemCommandValidator : AbstractValidator<UpdateTodoItemCommand>
{
    public UpdateTodoItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class UpdateTodoItemCommandHandler(IApplicationDbContext context) : ICommandHandler<UpdateTodoItemCommand>
{
    public async Task Handle(UpdateTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await context.TodoItems.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (todoItem is null)
        {
            throw new TodoItemNotFoundException(command.Id);
        }

        todoItem.Complete(command.IsDone);

        await context.SaveChangesAsync(cancellationToken);
    }
}
