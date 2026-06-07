using Dddify.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify.EntityFrameworkCore;

public class DbContextUnitOfWorkOptionsExtension<TContextService, TContextImplementation> : IOptionsExtension
    where TContextImplementation : DbContext, TContextService
{
    private readonly Action<IServiceProvider, DbContextOptionsBuilder>? _optionsAction;

    public DbContextUnitOfWorkOptionsExtension(Action<DbContextOptionsBuilder>? optionsAction)
    {
        _optionsAction = optionsAction is null
            ? null
            : (_, optionsBuilder) => optionsAction(optionsBuilder);
    }

    public DbContextUnitOfWorkOptionsExtension(Action<IServiceProvider, DbContextOptionsBuilder> optionsAction)
    {
        _optionsAction = optionsAction ?? throw new ArgumentNullException(nameof(optionsAction));
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<ISaveChangesInterceptor, ApplyEntityStateInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();
        services.AddScoped<IUnitOfWork, UnitOfWork<TContextImplementation>>();

        services.AddDbContext<TContextService, TContextImplementation>((sp, optionsBuilder) =>
        {
            optionsBuilder.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            _optionsAction?.Invoke(sp, optionsBuilder);
        });
    }
}
