using Dddify;
using Dddify.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseDddify(this IApplicationBuilder app)
    {
        ArgumentNullException.ThrowIfNull(app);

        app.UseRequestLocalization();

        return app;
    }

    internal static IApplicationBuilder UseRequestLocalization(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<AppLocalizationOptions>>()
            .Value;

        if (options.SupportedCultures.Any())
        {
            var localizationOptions = new RequestLocalizationOptions()
                .AddSupportedCultures(options.SupportedCultures)
                .AddSupportedUICultures(options.SupportedCultures)
                .SetDefaultCulture(string.IsNullOrEmpty(options.DefaultCulture) ? options.SupportedCultures[0] : options.DefaultCulture);

            localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

            app.UseRequestLocalization(localizationOptions);
        }

        return app;
    }
}
