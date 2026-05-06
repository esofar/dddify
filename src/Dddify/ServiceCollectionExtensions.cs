using Dddify.Dependency;
using Dddify.Messaging.Behaviors;
using FluentValidation;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using System.Reflection;

namespace Dddify;

/// <summary>
/// Provides extension methods for registering Dddify services and conventions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Dddify core services, MediatR behaviors, validators, and convention-based service scanning.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <param name="optionsAction">An optional action used to configure Dddify registration options.</param>
    /// <returns>The configured service collection.</returns>
    public static IServiceCollection AddDddify(this IServiceCollection services, Action<DddifyOptionsBuilder>? optionsAction = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var optionsBuilder = new DddifyOptionsBuilder();
        optionsAction?.Invoke(optionsBuilder);

        var options = optionsBuilder.Options;
        options.Assemblies ??= GetDefaultAssemblies();

        var mapsterConfig = new TypeAdapterConfig();
        mapsterConfig.Scan(options.Assemblies);
        options.ConfigureMapster?.Invoke(mapsterConfig);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, Mapper>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(options.Assemblies);

            if (options.ValidationBehaviorEnabled)
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));

            if (options.UnitOfWorkBehaviorEnabled)
                cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));

            options.ConfigureMediatR?.Invoke(cfg);
        });

        services.Scan(cfg =>
        {
            cfg.FromAssemblies(options.Assemblies)
                .RegisterDependencies()
                .RegisterDomainServices()
                .RegisterRepositories()
                .RegisterValidators();

            options.ConfigureScrutor?.Invoke(cfg);
        });

        foreach (var extension in options.Extensions)
        {
            extension.ConfigureServices(services);
        }

        return services;
    }

    /// <summary>
    /// Resolves the default set of project assemblies used for scanning when no assemblies are explicitly configured.
    /// </summary>
    /// <returns>The discovered project assemblies.</returns>
    private static Assembly[] GetDefaultAssemblies()
    {
        var dependencyContext = DependencyContext.Default;
        ArgumentNullException.ThrowIfNull(dependencyContext);

        return [.. dependencyContext.CompileLibraries
            .Where(library => library.Type == "project")
            .Select(library => Assembly.Load(library.Name))
            .Distinct()];
    }
}
