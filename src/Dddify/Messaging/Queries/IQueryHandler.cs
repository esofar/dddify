using MediatR;

namespace Dddify.Messaging.Queries;

/// <summary>
/// Defines a handler for a query.
/// </summary>
/// <typeparam name="TQuery">The type of query being handled.</typeparam>
/// <typeparam name="TResult">The type of result from the handler.</typeparam>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    /// <summary>
    /// Handles a query.
    /// </summary>
    /// <param name="query">The query.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The result from the query.</returns>
    new Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
}