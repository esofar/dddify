using Dddify.EntityFrameworkCore;
using Dddify.Guids;
using Dddify.Messaging.Behaviours;
using Dddify.Timing;
using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;

namespace Dddify;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDddify(this IServiceCollection services, Action<DddifyOptionsBuilder>? optionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var optionsBuilder = new DddifyOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);
        var options = optionsBuilder.Options;

        var assemblies = DependencyContext.Default.GetProjectAssemblies();

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            if (options.ValidationBehaviourEnabled)
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));

            if (options.UnitOfWorkBehaviorEnabled)
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));

            options.ConfigureMediatr?.Invoke(cfg);
        });

        services.Scan(scan =>
        {
            var selector = scan.FromAssemblies(assemblies);

            selector.RegisterAttributeDependencies()
                .RegisterDomainServices()
                .RegisterRepositories();

            options.ConfigureScrutor?.Invoke(selector);
        });

        TypeAdapterConfig.GlobalSettings.Scan(assemblies);

        services.AddSingleton<IClock>(_ => new Clock(options.DateTimeKind));
        services.AddTransient<IGuidGenerator>(_ => new SequentialGuidGenerator(options.SequentialGuidType));

        foreach (var extension in options.Extensions)
        {
            extension.ConfigureServices(services);
        }

        return services;
    }

}
