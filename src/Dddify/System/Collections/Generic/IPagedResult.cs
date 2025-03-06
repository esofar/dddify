namespace System.Collections.Generic;

/// <summary>
/// Represents a paged result containing a collection of items of type T.
/// </summary>
/// <typeparam name="T">The type of items in the paged result.</typeparam>
public interface IPagedResult<T>
{
    /// <summary>
    /// Gets the total number of items available.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the collection of items for the current page.
    /// </summary>
    IEnumerable<T> Items { get; }
}