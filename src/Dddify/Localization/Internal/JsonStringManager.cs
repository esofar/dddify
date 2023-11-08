using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace Dddify.Localization.Internal;

public class JsonStringManager
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _resourcesCache = new();

    public JsonStringManager(string resourcesPath, string resourceName)
    {
        ResourcesPath = resourcesPath;
        ResourceName = resourceName;
    }

    public string ResourcesPath { get; }

    public string ResourceName { get; }

    public virtual ConcurrentDictionary<string, string>? GetResourceSet(CultureInfo culture, bool tryParents)
    {
        TryLoadResourceSet(culture);

        if (!_resourcesCache.ContainsKey(culture.Name))
        {
            return null;
        }

        if (tryParents)
        {
            var allResources = new ConcurrentDictionary<string, string>();

            do
            {
                if (_resourcesCache.TryGetValue(culture.Name, out var resources))
                {
                    foreach (var entry in resources)
                    {
                        allResources.TryAdd(entry.Key, entry.Value);
                    }
                }

                culture = culture.Parent;
            } while (culture != CultureInfo.InvariantCulture);

            return allResources;
        }
        else
        {
            _resourcesCache.TryGetValue(culture.Name, out var resources);

            return resources;
        }
    }

    public virtual string? GetString(string name)
    {
        var culture = CultureInfo.CurrentUICulture;

        GetResourceSet(culture, tryParents: true);

        if (_resourcesCache.IsEmpty)
        {
            return null;
        }

        do
        {
            if (_resourcesCache.ContainsKey(culture.Name))
            {
                if (_resourcesCache[culture.Name].TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            culture = culture.Parent;
        } while (culture != culture.Parent);

        return null;
    }

    public virtual string? GetString(string name, CultureInfo culture)
    {
        GetResourceSet(culture, tryParents: true);

        if (_resourcesCache.IsEmpty)
        {
            return null;
        }

        if (!_resourcesCache.ContainsKey(culture.Name))
        {
            return null;
        }

        return _resourcesCache[culture.Name].TryGetValue(name, out var value)
            ? value
            : null;
    }

    private void TryLoadResourceSet(CultureInfo culture)
    {
        var file = string.IsNullOrEmpty(ResourceName)
            ? Path.Combine(ResourcesPath, $"{culture.Name}.json")
            : Path.Combine(ResourcesPath, culture.Name, $"{ResourceName}.json");

        var resources = LoadJsonResources(file);
        _resourcesCache.TryAdd(culture.Name, new(resources.ToDictionary(r => r.Key, r => r.Value)));
    }

    private static IDictionary<string, string> LoadJsonResources(string filePath)
    {
        var resources = new Dictionary<string, string>();

        if (File.Exists(filePath))
        {
            var content = File.ReadAllText(filePath, Encoding.UTF8);

            if (!string.IsNullOrWhiteSpace(content))
            {
                resources = JsonSerializer.Deserialize<Dictionary<string, string>>(content.Trim())!;
            }
        }

        return resources;
    }
}