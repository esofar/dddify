using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Reflection;

namespace Dddify.Localization;

/// <summary>
/// An <see cref="IStringLocalizerFactory"/> that creates instances of <see cref="JsonStringLocalizer"/>.
/// </summary>
public class JsonStringLocalizerFactory : IStringLocalizerFactory
{
    private readonly IResourceNamesCache _resourceNamesCache = new ResourceNamesCache();
    private readonly ConcurrentDictionary<string, JsonStringLocalizer> _localizerCache = new();
    private readonly string _resourcesRelativePath;
    private readonly ILoggerFactory _loggerFactory;

    public JsonStringLocalizerFactory(IOptions<JsonLocalizationOptions> localizationOptions, ILoggerFactory loggerFactory)
    {
        ArgumentNullException.ThrowIfNull(localizationOptions);
        ArgumentNullException.ThrowIfNull(loggerFactory);

        _resourcesRelativePath = localizationOptions.Value.ResourcesPath ?? string.Empty;
        _loggerFactory = loggerFactory;
    }

    public IStringLocalizer Create(Type resourceSource)
    {
        ArgumentNullException.ThrowIfNull(resourceSource);

        if (!_localizerCache.TryGetValue(resourceSource.AssemblyQualifiedName!, out var localizer))
        {
            var resourceName = resourceSource.Name;
            var resourcesPath = GetResourcePath();

            localizer = CreateJsonStringLocalizer(resourcesPath, resourceName);

            _localizerCache[resourceSource.AssemblyQualifiedName!] = localizer;
        }

        return localizer;
    }

    public IStringLocalizer Create(string baseName, string location)
    {
        ArgumentNullException.ThrowIfNull(nameof(baseName));
        ArgumentNullException.ThrowIfNull(nameof(location));

        return _localizerCache.GetOrAdd($"B={baseName},L={location}", _ =>
        {
            var assemblyName = new AssemblyName(location);
            var assembly = Assembly.Load(assemblyName);
            var resourcesPath = GetResourcePath(assembly);

            return CreateJsonStringLocalizer(resourcesPath, baseName);
        });
    }

    protected virtual JsonStringLocalizer CreateJsonStringLocalizer(string resourcesPath, string resourceName)
    {
        return new(
            new(resourcesPath, resourceName),
            resourceName,
            _resourceNamesCache,
            _loggerFactory.CreateLogger<JsonStringLocalizer>());
    }

    private string GetResourcePath(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetEntryAssembly();
        return Path.Combine(Path.GetDirectoryName(assembly!.Location)!, _resourcesRelativePath);
    }
}