using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

public static class QueryableExtensions
{
    /// <summary>
    /// Converts an <see cref="IQueryable{T}"/> to a paged result by performing pagination and returning the result as an <see cref="IPagedResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to convert.</param>
    /// <param name="page">The page number.</param>
    /// <param name="size">The number of records per page.</param>
    /// <returns>An <see cref="IPagedResult{T}"/> representing the paged result.</returns>
    public static IPagedResult<T> ToPagedResult<T>(this IQueryable<T> query, int page, int size)
    {
        var total = query.Count();
        var items = query.Skip((page - 1) * size).Take(size).ToList();

        return new PagedResult<T>(total, items);
    }

    /// <summary>
    /// Asynchronously converts an <see cref="IQueryable{T}"/> to a paged result by performing pagination and returning the result as an <see cref="IPagedResult{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to convert.</param>
    /// <param name="page">The page number.</param>
    /// <param name="size">The number of records per page.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IPagedResult{T}"/> representing the paged result.</returns>
    public static async Task<IPagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, int page, int size, CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return new PagedResult<T>(total, items);
    }

    /// <summary>
    /// Performs pagination on an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to paginate.</param>
    /// <param name="page">The page number.</param>
    /// <param name="size">The number of records per page.</param>
    /// <returns>The paginated <see cref="IQueryable{T}"/>.</returns>
    public static IQueryable<T> Paged<T>(this IQueryable<T> query, int page, int size)
    {
        return query.Skip((page - 1) * size).Take(size);
    }

    /// <summary>
    /// Filters the <see cref="IQueryable{T}"/> based on the specified <paramref name="predicate"/> if the specified <paramref name="condition"/> is true.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to filter.</param>
    /// <param name="condition">The condition indicating whether to apply the filter.</param>
    /// <param name="predicate">An expression representing the filter predicate.</param>
    /// <returns>The filtered <see cref="IQueryable{T}"/> if the condition is true; otherwise, the original <see cref="IQueryable{T}"/>.</returns>
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    /// Includes the specified navigation property in the query if the specified <paramref name="condition"/> is true.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to include the navigation property in.</param>
    /// <param name="condition">The condition indicating whether to include the navigation property.</param>
    /// <param name="path">An expression representing the navigation property to include.</param>
    /// <returns>The <see cref="IQueryable{T}"/> with the navigation property included if the condition is true; otherwise, the original <see cref="IQueryable{T}"/>.</returns>
    public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> query, bool condition, Expression<Func<T, TProperty>> path)
        where T : class
    {
        return condition ? query.Include(path) : query;
    }
}