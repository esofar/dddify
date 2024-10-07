using Dddify.EntityFrameworkCore;
using Dddify.Guids;
using Dddify.Identity;
using Dddify.Timing;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Dddify;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDddify(this IServiceCollection services, Action<DddifyOptionsBuilder>? optionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new DddifyOptionsBuilder();

        optionsAction?.Invoke(builder);
        var options = builder.Build();

        var assemblies = DependencyContext.Default.GetProjectAssemblies();

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            options.OpenBehaviors.ForEach(behavior => cfg.AddOpenBehavior(behavior));
        });

        services.Scan(scan => scan.FromAssemblies(assemblies)
                .RegisterDomainServices()
                .RegisterDependencyServices());

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assemblies);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddSingleton<IClock>(_ => new Clock(options.DateTimeKind));

        services.AddTransient<IGuidGenerator>(_ => new SequentialGuidGenerator(options.SequentialGuidType));
        
        services.AddScoped<InternalInterceptor>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextUser>();

        options.Extensions.ForEach(extension => extension.ConfigureServices(services));

        return services;
    }


}