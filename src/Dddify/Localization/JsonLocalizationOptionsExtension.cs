using Dddify;
using Dddify.Localization;
using Dddify.Localization.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public class JsonLocalizationOptionsExtension : IOptionsExtension
{
    private const string _defaultConfigSectionPath = "JsonLocalization";
    private readonly string _configSectionPath;
    private readonly Action<JsonLocalizationOptions>? _configure;

    public JsonLocalizationOptionsExtension(string configSectionPath, Action<JsonLocalizationOptions>? configure)
    {
        _configSectionPath = configSectionPath;
        _configure = configure;
    }

    public JsonLocalizationOptionsExtension(Action<JsonLocalizationOptions>? configure)
        : this(_defaultConfigSectionPath, configure)
    {
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<JsonLocalizationOptions>().BindConfiguration(_configSectionPath);
        services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer));
        services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, RequestLocalizationConfigureOptions>();

        if (_configure != null)
        {
            services.PostConfigure(_configure);
        }
    }
}