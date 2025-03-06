using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Dddify.Localization;

public class RequestLocalizationConfigureOptions(IOptions<JsonLocalizationOptions> options) : IConfigureOptions<RequestLocalizationOptions>
{
    private readonly JsonLocalizationOptions _options = options.Value;

    public void Configure(RequestLocalizationOptions options)
    {
        if (_options.SupportedCultures.Length != 0)
        {
            options.AddSupportedCultures(_options.SupportedCultures)
                .AddSupportedUICultures(_options.SupportedCultures)
                .SetDefaultCulture(string.IsNullOrEmpty(_options.DefaultCulture) ? _options.SupportedCultures[0] : _options.DefaultCulture);
        }
    }
}