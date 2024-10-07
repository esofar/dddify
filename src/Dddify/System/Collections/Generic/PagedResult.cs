namespace System.Collections.Generic;

public class PagedResult<T>(int total, IEnumerable<T> items) : IPagedResult<T>
{
    public int Total => total;

    public IEnumerable<T> Items => items;
}