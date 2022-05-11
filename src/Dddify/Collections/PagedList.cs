namespace System.Collections.Generic;

public class PagedList<T> : IPagedList<T>
{
    public PagedList()
    {

    }

    public PagedList(int total, IEnumerable<T> items)
    {
        Total = total;
        Items = items;
    }

    public int Total { get; }

    public IEnumerable<T> Items { get; } = Enumerable.Empty<T>();
}
