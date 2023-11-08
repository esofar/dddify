using Dddify.AspNetCore.Results;
using Dddify.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public class ApiResultWrapperOptionsExtension : IOptionsExtension
{
    private readonly Action<ApiResultWrapperOptions>? _configure;

    public ApiResultWrapperOptionsExtension(Action<ApiResultWrapperOptions>? configure)
    {
        _configure = configure;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var options = new ApiResultWrapperOptions();
        _configure?.Invoke(options);

        services.AddTransient<IApiResultWrapper, ApiResultWrapper>();

        // Customise default API behaviour.
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        // Customise default MVC behaviour.
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(typeof(ApiExceptionFilter));
            options.Filters.Add(typeof(ApiResultFilter));
        });

        if (_configure != null)
        {
            services.PostConfigure(_configure);
        }
    }
}