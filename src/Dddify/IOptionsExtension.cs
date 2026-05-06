using Microsoft.Extensions.DependencyInjection;

namespace Dddify;

/// <summary>
/// Defines a modular service-registration extension that can contribute services to Dddify during startup.
/// </summary>
public interface IOptionsExtension
{
    /// <summary>
    /// Adds the services required by the extension to the specified collection.
    /// </summary>
    /// <param name="services">The service collection being configured.</param>
    void ConfigureServices(IServiceCollection services);
}
