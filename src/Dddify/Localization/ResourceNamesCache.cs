using System.Collections.Concurrent;

namespace Dddify.Localization
{
    /// <summary>
    /// An implementation of <see cref="IResourceNamesCache"/> backed by a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
    /// </summary>
    public class ResourceNamesCache : IResourceNamesCache
    {
        private readonly ConcurrentDictionary<string, IList<string>?> _cache = new();

        public ResourceNamesCache()
        {
        }

        public IList<string>? GetOrAdd(string name, Func<string, IList<string>?> valueFactory)
        {
            return _cache.GetOrAdd(name, valueFactory);
        }
    }
}