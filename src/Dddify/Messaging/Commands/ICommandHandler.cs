using MediatR;

namespace Dddify.Messaging.Commands;

/// <summary>
/// Defines a handler for a command with a void result.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Handles a command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    new Task Handle(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a command.
/// </summary>
/// <typeparam name="TCommand">The type of command being handled.</typeparam>
/// <typeparam name="TResult">The type of result from the handler.</typeparam>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles a command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result from the command.</returns>
    new Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
}