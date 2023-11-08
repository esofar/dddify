namespace System.Collections.Generic;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action, nameof(action));

        foreach (var item in source)
        {
            action(item);
        }
    }

    public static IPagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int page, int size)
    {
        var total = source.Count();
        var items = source.Skip((page - 1) * size).Take(size).ToList();

        return new PagedResult<T>(total, items);
    }
}