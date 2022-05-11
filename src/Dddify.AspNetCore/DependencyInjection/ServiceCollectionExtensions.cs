using Dddify.Guids;
using Dddify.Timing;
using System;
using Dddify.DependencyInjection;
using Dddify;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Dddify services in an <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Dddify services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="builderAction">An <see cref="Action{DddifyBuilder}"/> to configure the <see cref="DddifyBuilder"/>.</param>
    /// <returns>An <see cref="IDddifyBuilder"/>that can be used to further configure the Dddify services.</returns>
    public static IDddifyBuilder AddDddify(this IServiceCollection services, Action<IDddifyBuilder> builderAction)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(builderAction);

        services.AddOptions<SequentialGuidGeneratorOptions>();
        services.AddOptions<ClockOptions>();

        services.Scan(scan => scan.FromApplicationDependencies()
                .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
                    .AsImplementedInterfaces().WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
                    .AsImplementedInterfaces().WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
                    .AsImplementedInterfaces().WithSingletonLifetime());

        var builder = new DddifyBuilder(services);
        builderAction.Invoke(builder);

        return builder;
    }
}
