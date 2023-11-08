using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Dddify.Localization;

internal class RequestLocalizationConfigureOptions : IConfigureOptions<RequestLocalizationOptions>
{
    private readonly JsonLocalizationOptions _options;

    public RequestLocalizationConfigureOptions(IOptions<JsonLocalizationOptions> options)
    {
        _options = options.Value;
    }

    public void Configure(RequestLocalizationOptions options)
    {
        if (_options.SupportedCultures.Any())
        {
            options.AddSupportedCultures(_options.SupportedCultures)
                .AddSupportedUICultures(_options.SupportedCultures)
                .SetDefaultCulture(string.IsNullOrEmpty(_options.DefaultCulture) ? _options.SupportedCultures[0] : _options.DefaultCulture);
        }
    }
}