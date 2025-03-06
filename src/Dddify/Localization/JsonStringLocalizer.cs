using Dddify.Localization.Internal;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Globalization;
using System.Resources;

namespace Dddify.Localization;

/// <summary>
/// An <see cref="IStringLocalizer"/> that uses the <see cref="JsonStringManager"/> to provide localized strings.
/// </summary>
public partial class JsonStringLocalizer : IStringLocalizer
{
    private readonly ConcurrentDictionary<string, object> _missingManifestCache = new();
    private readonly JsonStringManager _jsonStringManager;
    private readonly IJsonStringProvider _jsonStringProvider;
    private readonly string _resourceBaseName;
    private readonly ILogger _logger;

    public JsonStringLocalizer(
        JsonStringManager jsonStringManager,
        string baseName,
        IResourceNamesCache resourceNamesCache,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(jsonStringManager);
        ArgumentNullException.ThrowIfNull(baseName);
        ArgumentNullException.ThrowIfNull(resourceNamesCache);
        ArgumentNullException.ThrowIfNull(logger);

        _jsonStringManager = jsonStringManager;
        _resourceBaseName = baseName;
        _jsonStringProvider = new JsonStringProvider(resourceNamesCache, jsonStringManager);
        _logger = logger;
    }

    public LocalizedString this[string name]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);

            var value = GetStringSafely(name, null);

            return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(name);

            var format = GetStringSafely(name, null);
            var value = string.Format(format ?? name, arguments);

            return new LocalizedString(name, value, resourceNotFound: format == null, searchedLocation: _resourceBaseName);
        }
    }

    public virtual IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        GetAllStrings(includeParentCultures, CultureInfo.CurrentUICulture);

    protected IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var resourceNames = includeParentCultures
            ? GetResourceNamesFromCultureHierarchy(culture)
            : _jsonStringProvider.GetAllResourceStrings(culture, true);

        if (resourceNames != null)
        {
            foreach (var name in resourceNames)
            {
                var value = GetStringSafely(name, culture);
                yield return new LocalizedString(name, value ?? name, resourceNotFound: value == null, searchedLocation: _resourceBaseName);
            }
        }
    }

    protected string? GetStringSafely(string name, CultureInfo? culture)
    {
        ArgumentNullException.ThrowIfNull(name);

        var keyCulture = culture ?? CultureInfo.CurrentUICulture;
        var cacheKey = $"name={name}&culture={keyCulture.Name}";

        _logger.LogDebug($"{nameof(JsonStringLocalizer)} searched for '{{Key}}' in '{{LocationSearched}}' with culture '{{Culture}}'.", name, _resourceBaseName, keyCulture);

        if (_missingManifestCache.ContainsKey(cacheKey))
        {
            return null;
        }

        try
        {
            return culture == null
                        ? _jsonStringManager.GetString(name)
                        : _jsonStringManager.GetString(name, culture);
        }
        catch (MissingManifestResourceException)
        {
            _missingManifestCache.TryAdd(cacheKey, null!);
            return null;
        }
    }

    private IEnumerable<string>? GetResourceNamesFromCultureHierarchy(CultureInfo startingCulture)
    {
        var currentCulture = startingCulture;
        var resourceNames = new HashSet<string>();

        while (currentCulture != currentCulture.Parent)
        {
            var cultureResourceNames = _jsonStringProvider.GetAllResourceStrings(currentCulture, false);

            if (cultureResourceNames != null)
            {
                foreach (var resourceName in cultureResourceNames)
                {
                    resourceNames.Add(resourceName);
                }
            }

            currentCulture = currentCulture.Parent;
        }

        return resourceNames?.ToList();
    }
}