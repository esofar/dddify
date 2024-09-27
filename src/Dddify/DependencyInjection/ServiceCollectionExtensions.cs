using Dddify.Dependency;
using Dddify.DependencyInjection;
using Dddify.Domain;
using Dddify.EntityFrameworkCore;
using Dddify.Guids;
using Dddify.Identity;
using Dddify.Timing;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyModel;
using Scrutor;
using System;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDddify(this IServiceCollection services, Action<DddifyOptionsBuilder>? optionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var builder = new DddifyOptionsBuilder();

        optionsAction?.Invoke(builder);

        var options = builder.Build();

        var assemblies = DependencyContext.Default!.GetInternalAssemblies().ToArray();

        services.AddHttpContextAccessor();

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
            options.OpenBehaviors.ForEach(behavior => cfg.AddOpenBehavior(behavior));
        });

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assemblies);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        services.AddSingleton<IClock>(_ => new Clock(options.DateTimeKind));
        services.AddTransient<IGuidGenerator>(_ => new SequentialGuidGenerator(options.SequentialGuidType));
        services.AddScoped<ICurrentUser, HttpContextUser>();
        services.AddScoped<InternalInterceptor>();

        options.Extensions.ForEach(extension => extension.ConfigureServices(services));

        services.Scan(scan => scan.FromAssemblies(assemblies)
                .RegisterDomainServices()
                .RegisterDependencyServices());

        return services;
    }


}