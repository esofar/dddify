using Dddify;
using Dddify.Users;

namespace Microsoft.Extensions.DependencyInjection;

public class CurrentUserOptionsExtension<TCurrentUserImplementation>(
    Type? extendedCurrentUserServiceType = null,
    Type? extendedCurrentUserImplementationType = null) : IOptionsExtension
    where TCurrentUserImplementation : class, ICurrentUser
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ICurrentUser, TCurrentUserImplementation>();

        if (extendedCurrentUserServiceType != null && extendedCurrentUserImplementationType != null)
        {
            services.AddScoped(extendedCurrentUserServiceType, extendedCurrentUserImplementationType);
        }
    }
}