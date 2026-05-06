using Dddify.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Provides EF Core-oriented query extensions for paging and conditional composition.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Materializes a query into a paged result.
    /// </summary>
    /// <typeparam name="T">The element type returned by the query.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="page">The one-based page number.</param>
    /// <param name="size">The number of items per page.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    /// <returns>A paged result containing the total count and the items for the requested page.</returns>
    public static async Task<IPagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int page, int size, CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Paged(page, size).ToListAsync(cancellationToken);

        return new PagedResult<T>(total, items);
    }

    /// <summary>
    /// Applies paging to a query without materializing it.
    /// </summary>
    /// <typeparam name="T">The element type returned by the query.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="page">The one-based page number.</param>
    /// <param name="size">The number of items per page.</param>
    /// <returns>A query representing the requested page.</returns>
    public static IQueryable<T> Paged<T>(this IQueryable<T> query, int page, int size)
    {
        return query.Skip((page - 1) * size).Take(size);
    }

    /// <summary>
    /// Applies a filter only when the specified condition is satisfied.
    /// </summary>
    /// <typeparam name="T">The element type returned by the query.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="condition">Whether the predicate should be applied.</param>
    /// <param name="predicate">The filter predicate.</param>
    /// <returns>The filtered query when <paramref name="condition"/> is <see langword="true"/>; otherwise, the original query.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Applies an include only when the specified condition is satisfied.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TProperty">The navigation property type.</typeparam>
    /// <param name="query">The source query.</param>
    /// <param name="condition">Whether the include should be applied.</param>
    /// <param name="path">The navigation property expression.</param>
    /// <returns>The query with the include applied when <paramref name="condition"/> is <see langword="true"/>; otherwise, the original query.</returns>
    public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> query, bool condition, Expression<Func<T, TProperty>> path)
        where T : class
    {
        return condition ? query.Include(path) : query;
    }
}
