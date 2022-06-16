using Dddify;
using Dddify.AspNetCore.ApiResult;
using Dddify.AspNetCore.Localization;
using Dddify.DependencyInjection;
using Dddify.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extensions for configuring Dddify using an <see cref="IDddifyBuilder"/>.
/// </summary>
public static class DddifyBuilderExtensions
{
    public static IDddifyBuilder AddApiResult(this IDddifyBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.Configure<MvcOptions>(options =>
        {
            options.Filters.Add(typeof(ApiExceptionFilter));
            options.Filters.Add(typeof(ApiResultFilter));
        });

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddTransient<IApiResultWrapper, ApiResultWrapper>();

        return builder;
    }

    public static IDddifyBuilder AddLocalization(this IDddifyBuilder builder, Action<AppLocalizationOptions> setupAction)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(setupAction);

        var options = new AppLocalizationOptions();
        setupAction?.Invoke(options);

        builder.Services.AddOptions<AppLocalizationOptions>();
        builder.Services.AddTransient<ISharedStringLocalizer, SharedStringLocalizer>();
        builder.Services.AddLocalization(c => c.ResourcesPath = options.ResourcesPath);

        return builder;
    }
}
