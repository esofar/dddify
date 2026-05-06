using System.Globalization;
using System.Resources;

namespace Dddify.Localization.Internal;

public class JsonStringProvider(IResourceNamesCache resourceNamesCache, JsonStringManager jsonStringManager) : IJsonStringProvider
{
    private string GetResourceCacheKey(CultureInfo culture)
    {
        var resourceName = jsonStringManager.ResourceName;

        return $"culture={culture.Name}&resourceName={resourceName}";
    }

    public IList<string>? GetAllResourceStrings(CultureInfo culture, bool throwOnMissing)
    {
        var cacheKey = GetResourceCacheKey(culture);

        return resourceNamesCache.GetOrAdd(cacheKey, _ =>
        {
            var resourceSet = jsonStringManager.GetResourceSet(culture, tryParents: false);

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

            return [.. resourceSet.Select(entry => entry.Key)];
        });
    }
}