namespace System.Collections.Generic;

public class PagedResult<T> : IPagedResult<T>
{
    public PagedResult(int total, IEnumerable<T> items)
    {
        Total = total;
        Items = items;
    }

    public int Total { get; }

    public IEnumerable<T> Items { get; }
}