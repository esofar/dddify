using Dddify.Users.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify.Users;

/// <summary>
/// Registers Dddify current-user services.
/// </summary>
internal sealed class CurrentUserOptionsExtension(Action<CurrentUserOptions>? configure = null) : IOptionsExtension
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddOptions<CurrentUserOptions>();

        if (configure != null)
        {
            services.Configure(configure);
        }

        var options = new CurrentUserOptions();
        configure?.Invoke(options);

        services.AddScoped(typeof(ICurrentUserProvider), options.ProviderType);
        services.AddScoped<ICurrentUser, CurrentUser>();

        foreach (var extendedCurrentUserType in options.ExtendedCurrentUserTypes)
        {
            services.AddScoped(extendedCurrentUserType.Key, extendedCurrentUserType.Value);
        }
    }
}
