using Dddify.Timing.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify.Timing;

/// <summary>
/// Registers Dddify timing services.
/// </summary>
public class TimingOptionsExtension(Action<TimingOptions>? configure = null) : IOptionsExtension
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<TimingOptions>();

        if (configure != null)
        {
            services.Configure(configure);
        }

        var options = new TimingOptions();
        configure?.Invoke(options);

        services.AddScoped<IClock, Clock>();
        services.AddScoped<ITimeZoneResolver, TimeZoneResolver>();
        services.AddScoped<ICurrentTimeZone, CurrentTimeZone>();
        services.AddScoped<IDateTimeConverter, DateTimeConverter>();
        services.AddScoped(typeof(ITimeZoneIdProvider), options.TimeZoneIdProviderType);
    }
}
