using Dddify.SharedKernel;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Dddify.Dependency;

/// <summary>
/// Provides helper methods for applying Dddify's Scrutor-based service registration conventions.
/// </summary>
public static class ScrutorExtensions
{
    /// <summary>
    /// Applies the specified registration mode and lifetime to the selected services.
    /// </summary>
    /// <param name="selector">The Scrutor service type selector.</param>
    /// <param name="registrationMode">Determines how implementations are exposed.</param>
    /// <param name="lifetime">The service lifetime to apply.</param>
    /// <returns>
    /// The implementation selector with the configured registration mode and lifetime.
    /// </returns>
    public static IImplementationTypeSelector Register(
        this IServiceTypeSelector selector,
        RegistrationMode registrationMode,
        ServiceLifetime lifetime)
    {
        var lifetimeSelector = registrationMode switch
        {
            RegistrationMode.AsSelf => selector.AsSelf(),
            RegistrationMode.AsMatchingInterface => selector.AsMatchingInterface(),
            RegistrationMode.AsImplementedInterfaces => selector.AsImplementedInterfaces(),
            _ => throw new ArgumentOutOfRangeException(nameof(registrationMode), registrationMode, "Unsupported registration mode.")
        };

        return lifetime switch
        {
            ServiceLifetime.Singleton => lifetimeSelector.WithSingletonLifetime(),
            ServiceLifetime.Scoped => lifetimeSelector.WithScopedLifetime(),
            ServiceLifetime.Transient => lifetimeSelector.WithTransientLifetime(),
            _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unsupported service lifetime.")
        };
    }

    /// <summary>
    /// Registers types implementing <see cref="IDomainService"/> using Dddify's default domain service convention.
    /// </summary>
    /// <param name="selector">The Scrutor implementation selector.</param>
    /// <returns>The selector for further chained registrations.</returns>
    internal static IImplementationTypeSelector RegisterDomainServices(this IImplementationTypeSelector selector)
    {
        return selector.AddClasses(classes => classes.AssignableTo<IDomainService>())
            .Register(RegistrationMode.AsSelf, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Registers repository implementations using the matching interface convention.
    /// </summary>
    /// <param name="selector">The Scrutor implementation selector.</param>
    /// <returns>The selector for further chained registrations.</returns>
    internal static IImplementationTypeSelector RegisterRepositories(this IImplementationTypeSelector selector)
    {
        return selector.AddClasses(classes => classes.AssignableTo(typeof(IRepository<,>)))
            .Register(RegistrationMode.AsMatchingInterface, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Registers validator implementations using the implemented interface convention.
    /// </summary>
    /// <param name="selector">The Scrutor implementation selector.</param>
    /// <returns>The selector for further chained registrations.</returns>
    internal static IImplementationTypeSelector RegisterValidators(this IImplementationTypeSelector selector)
    {
        return selector.AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
            .Register(RegistrationMode.AsImplementedInterfaces, ServiceLifetime.Scoped);
    }

    /// <summary>
    /// Registers services annotated with Dddify dependency attributes.
    /// </summary>
    /// <param name="selector">The Scrutor implementation selector.</param>
    /// <returns>The selector for further chained registrations.</returns>
    internal static IImplementationTypeSelector RegisterDependencies(this IImplementationTypeSelector selector)
    {
        var serviceTypes = Enum.GetValues<RegistrationMode>().Cast<RegistrationMode>();

        var serviceLifetimes = new Dictionary<Type, ServiceLifetime>
        {
            { typeof(SingletonDependencyAttribute), ServiceLifetime.Singleton},
            { typeof(ScopedDependencyAttribute), ServiceLifetime.Scoped},
            { typeof(TransientDependencyAttribute), ServiceLifetime.Transient},
        };

        foreach (var serviceType in serviceTypes)
        {
            foreach (var serviceLifetime in serviceLifetimes)
            {
                selector.AddClasses(cls => cls.Where(type => type.GetCustomAttribute(serviceLifetime.Key) is DependencyAttribute dependency && dependency.RegistrationMode == serviceType))
                    .Register(serviceType, serviceLifetime.Value);
            }
        }

        return selector;
    }
}
