using Dddify.Messaging.Commands;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Domain.Entities;
using TodoListApp.Domain.Enums;
using TodoListApp.Domain.Exceptions;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Application.Commands;

public record CreateTodoItemCommand(string Text, string PriorityLevel) : ICommand;

public class CreateTodoItemCommandValidator : AbstractValidator<CreateTodoItemCommand>
{
    public CreateTodoItemCommandValidator()
    {
        RuleFor(c => c.Text).NotEmpty().MaximumLength(50);
        RuleFor(c => c.PriorityLevel).NotNull().IsEnumName(typeof(PriorityLevel), false);
    }
}

public class CreateTodoItemCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateTodoItemCommand>
{
    public async Task Handle(CreateTodoItemCommand command, CancellationToken cancellationToken)
    {
        if (await context.TodoItems.AnyAsync(c => c.Text == command.Text, cancellationToken))
        {
            throw new TodoItemDuplicateException(command.Text);
        }

        var todoItem = new TodoItem(command.Text, command.PriorityLevel.ToEnum<PriorityLevel>());

        await context.TodoItems.AddAsync(todoItem, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}
