using Dddify.Guids;
using Dddify.Identity;
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

        var assemblies = DependencyContext.Default.GetProjectAndDddifyAssemblies();

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);

            options.MediatrOptions?.Invoke(cfg);

            if (options.ValidationBehaviourEnabled)
                cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));

            if (options.UnitOfWorkBehaviorEnabled)
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
        });

        services.Scan(cfg =>
        {
            cfg.FromAssemblies(assemblies)
                .RegisterAttributeDependencies()
                .RegisterDomainServices()
                .RegisterRepositories();

            options.ScrutorOptions?.Invoke(cfg);
        });

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assemblies);

        services.AddSingleton<IClock>(_ => new Clock(options.DateTimeKind));

        services.AddTransient<IGuidGenerator>(_ => new SequentialGuidGenerator(options.SequentialGuidType));

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, HttpContextUser>();

        options.Extensions.ForEach(extension => extension.ConfigureServices(services));

        return services;
    }

}
