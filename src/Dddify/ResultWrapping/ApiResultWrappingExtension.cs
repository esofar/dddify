using Dddify.ResultWrapping.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify.ResultWrapping;

/// <summary>
/// Registers API result wrapping services, filters, and exception mapping for ASP.NET Core Web API.
/// </summary>
/// <param name="configure">An optional delegate used to configure <see cref="ApiResultWrappingOptions"/>.</param>
public class ApiResultWrappingExtension(Action<ApiResultWrappingOptions>? configure) : IOptionsExtension
{
    /// <summary>
    /// Registers API result wrapping services into the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOptions<ApiResultWrappingOptions>();

        if (configure != null)
        {
            services.Configure(configure);
        }

        var options = new ApiResultWrappingOptions();
        configure?.Invoke(options);

        services.AddSingleton(typeof(IBusinessExceptionResourceTypeResolver), options.BusinessException.ResourceTypeResolverType);
        services.AddTransient<IApiExceptionMapper, DefaultApiExceptionMapper>();

        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add<ApiExceptionFilter>();
            options.Filters.Add<ApiResultFilter>();
        });
    }
}
