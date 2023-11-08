using MediatR;

namespace Dddify.Messaging.Queries;

/// <summary>
/// Represents a query with a result.
/// </summary>
/// <typeparam name="TResult">The type of result.</typeparam>
public interface IQuery<out TResult> : IRequest<TResult>
{
}