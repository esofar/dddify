namespace Dddify.Primitives;

/// <summary>
/// Defines a paged result that contains the items for the current page and the total number of available items.
/// </summary>
/// <typeparam name="T">The type of items contained in the paged result.</typeparam>
public interface IPagedResult<T>
{
    /// <summary>
    /// Gets the total number of items available across all pages.
    /// </summary>
    int Total { get; }

    /// <summary>
    /// Gets the items contained in the current page.
    /// </summary>
    IEnumerable<T> Items { get; }
}
