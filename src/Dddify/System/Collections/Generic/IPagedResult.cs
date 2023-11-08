namespace System.Collections.Generic;

/// <summary>
/// This interface is defined to standardize to return a page of items to clients.
/// </summary>
/// <typeparam name="T">The type of the elements in the items.</typeparam>
public interface IPagedResult<T>
{
    /// <summary>
    /// The total count.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// The return a page of items.
    /// </summary>
    IEnumerable<T> Items { get; }
}