using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Entity Framework LINQ related extension methods.
/// </summary>
public static class QueryableExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int page, int size, CancellationToken cancellationToken = default)
    {
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
        return new PagedList<T>(total, items);
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> expression)
    {
        return condition ? query.Where(expression) : query;
    }

    /// <summary>
    /// Specifies the related objects to include in the query results.
    /// </summary>
    /// <param name="query">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
    /// <param name="condition">A boolean value to determine to include <paramref name="path"/> or not.</param>
    /// <param name="path">The type of navigation property being included.</param>
    public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> query, bool condition, Expression<Func<T, TProperty>> path)
        where T : class
    {
        return condition ? query.Include(path) : query;
    }
}
