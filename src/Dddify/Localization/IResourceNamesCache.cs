namespace Dddify.Localization
{
    /// <summary>
    /// Represents a cache of string names in resources.
    /// </summary>
    public interface IResourceNamesCache
    {
        /// <summary>
        /// Adds a set of resource names to the cache by using the specified function, if the name does not already exist.
        /// </summary>
        /// <param name="name">The resource name to add string names for.</param>
        /// <param name="valueFactory">The function used to generate the string names for the resource.</param>
        /// <returns>The string names for the resource.</returns>
        IList<string>? GetOrAdd(string name, Func<string, IList<string>?> valueFactory);
    }
}