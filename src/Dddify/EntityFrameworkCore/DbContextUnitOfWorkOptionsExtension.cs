using Dddify.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify.EntityFrameworkCore;

public class DbContextUnitOfWorkOptionsExtension<TContextService, TContextImplementation>(Action<DbContextOptionsBuilder>? optionsAction) : IOptionsExtension
    where TContextImplementation : DbContext, TContextService
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, ApplyEntityStateInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<IUnitOfWork, UnitOfWork<TContextImplementation>>();

        services.AddDbContext<TContextService, TContextImplementation>((sp, optionsBuilder) =>
        {
            optionsBuilder.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            optionsAction?.Invoke(optionsBuilder);
        });
    }
}
