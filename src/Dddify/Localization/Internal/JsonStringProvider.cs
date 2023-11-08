using System.Globalization;
using System.Resources;

namespace Dddify.Localization.Internal;

public class JsonStringProvider : IJsonStringProvider
{
    private readonly IResourceNamesCache _resourceNamesCache;
    private readonly JsonStringManager _jsonStringManager;

    public JsonStringProvider(IResourceNamesCache resourceNamesCache, JsonStringManager jsonStringManager)
    {
        _resourceNamesCache = resourceNamesCache;
        _jsonStringManager = jsonStringManager;
    }

    private string GetResourceCacheKey(CultureInfo culture)
    {
        var resourceName = _jsonStringManager.ResourceName;

        return $"culture={culture.Name}&resourceName={resourceName}";
    }

    public IList<string>? GetAllResourceStrings(CultureInfo culture, bool throwOnMissing)
    {
        var cacheKey = GetResourceCacheKey(culture);

        return _resourceNamesCache.GetOrAdd(cacheKey, _ =>
        {
            var resourceSet = _jsonStringManager.GetResourceSet(culture, tryParents: false);

            if (resourceSet == null)
            {
                if (throwOnMissing)
                {
                    throw new MissingManifestResourceException($"The manifest resource for the culture '{culture.Name}' is missing.");
                }
                else
                {
                    return null;
                }
            }

            return resourceSet.Select(entry => entry.Key).ToList();
        });
    }
}