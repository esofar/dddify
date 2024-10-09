using Dddify.Messaging.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Domain.Exceptions;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Commands;

public record DeleteTodoItemCommand(Guid Id) : ICommand;

public class DeleteTodoItemCommandValidator : AbstractValidator<DeleteTodoItemCommand>
{
    public DeleteTodoItemCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}

public class DeleteTodoItemCommandHandler(IApplicationDbContext context) : ICommandHandler<DeleteTodoItemCommand>
{
    public async Task Handle(DeleteTodoItemCommand command, CancellationToken cancellationToken)
    {
        var todoItem = await context.TodoItems.FirstOrDefaultAsync(c => c.Id == command.Id, cancellationToken);

        if (todoItem is null)
        {
            throw new TodoItemNotFoundException(command.Id);
        }

        context.TodoItems.Remove(todoItem);

        //await context.SaveChangesAsync(cancellationToken);
    }
}
