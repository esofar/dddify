using Dddify;
using Dddify.Localization;
using Dddify.Localization.Internal;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public class JsonLocalizationOptionsExtension(string configSectionPath, Action<JsonLocalizationOptions>? configure) : IOptionsExtension
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<JsonLocalizationOptions>().BindConfiguration(configSectionPath);
        services.TryAddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
        services.TryAddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
        services.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer));
        services.AddSingleton<IConfigureOptions<RequestLocalizationOptions>, RequestLocalizationConfigureOptions>();

        if (configure != null)
        {
            services.PostConfigure(configure);
        }
    }
}