using MediatR;

namespace Dddify.Messaging.Commands;

/// <summary>
/// Represents a command with no result.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Represents a command with a result.
/// </summary>
/// <typeparam name="TResult">The type of result.</typeparam>
public interface ICommand<out TResult> : IRequest<TResult>
{
}

public interface IUnitOfWorkCommand : ICommand
{
}

public interface IUnitOfWorkCommand<out TResult> : ICommand<TResult>
{
}