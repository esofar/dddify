using Dddify;
using Dddify.Domain;
using Dddify.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

public class DbContextOptionsExtension<TContextService, TContextImplementation>(Action<DbContextOptionsBuilder>? optionsAction) : IOptionsExtension
    where TContextImplementation : DbContext, TContextService
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IInterceptor, InternalInterceptor>();

        services.AddScoped<IUnitOfWork, UnitOfWork<TContextImplementation>>();

        services.AddDbContext<TContextService, TContextImplementation>((provider, optionsBuilder) =>
        {
            optionsBuilder.AddInterceptors(provider.GetServices<IInterceptor>());
            optionsAction?.Invoke(optionsBuilder);
        });
    }
}