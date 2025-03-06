using Dddify;
using Dddify.AspNetCore.Results;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection;

public class ApiResultWrapperOptionsExtension(Action<ApiResultWrapperOptions>? configure) : IOptionsExtension
{
    public void ConfigureServices(IServiceCollection services)
    {
        var options = new ApiResultWrapperOptions();
        configure?.Invoke(options);

        services.AddTransient<IApiResultWrapper, ApiResultWrapper>();

        //// Customise default API behaviour.
        //services.Configure<ApiBehaviorOptions>(options =>
        //{
        //    options.SuppressModelStateInvalidFilter = true;
        //});

        // Customise default MVC behaviour.
        services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(typeof(ApiExceptionFilter));
            options.Filters.Add(typeof(ApiResultFilter));
        });

        if (configure != null)
        {
            services.PostConfigure(configure);
        }
    }
}