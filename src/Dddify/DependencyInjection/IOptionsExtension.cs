using Microsoft.Extensions.DependencyInjection;

namespace Dddify.DependencyInjection;

public interface IOptionsExtension
{
    void ConfigureServices(IServiceCollection services);
}