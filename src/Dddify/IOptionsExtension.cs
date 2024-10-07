using Microsoft.Extensions.DependencyInjection;

namespace Dddify;

public interface IOptionsExtension
{
    void ConfigureServices(IServiceCollection services);
}