using Dddify.Dependency;
using Dddify.Domain;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Dddify;

public static class ScrutorExtensions
{
    public static IImplementationTypeSelector WithLifetime(this ILifetimeSelector lifetimeSelector, ServiceLifetime lifetime)
        => lifetime switch
        {
            ServiceLifetime.Singleton => lifetimeSelector.WithSingletonLifetime(),
            ServiceLifetime.Scoped => lifetimeSelector.WithScopedLifetime(),
            ServiceLifetime.Transient => lifetimeSelector.WithTransientLifetime(),
            _ => throw new ArgumentOutOfRangeException($"Unsupported lifetime: {lifetime}")
        };

    public static IImplementationTypeSelector Register(this IServiceTypeSelector serviceTypeSelector, RegistrationType registrationType, ServiceLifetime lifetime)
        => registrationType switch
        {
            RegistrationType.AsSelf => WithLifetime(serviceTypeSelector.AsSelf(), lifetime),
            RegistrationType.AsMatchingInterface => WithLifetime(serviceTypeSelector.AsMatchingInterface(), lifetime),
            RegistrationType.AsImplementedInterfaces => WithLifetime(serviceTypeSelector.AsImplementedInterfaces(), lifetime),
            _ => throw new ArgumentOutOfRangeException($"Unsupported registration type: {registrationType}")
        };

    public static IImplementationTypeSelector RegisterDomainServices(this IImplementationTypeSelector selector)
    {
        return selector
            .AddClasses(classes => classes.AssignableTo<IDomainService>())
            .Register(RegistrationType.AsSelf, ServiceLifetime.Scoped);
    }

    public static IImplementationTypeSelector RegisterRepositories(this IImplementationTypeSelector selector)
    {
        return selector
            .AddClasses(classes => classes.AssignableTo(typeof(IRepository<,>)))
            .Register(RegistrationType.AsMatchingInterface, ServiceLifetime.Scoped);
    }

    public static IImplementationTypeSelector RegisterAttributeDependencies(this IImplementationTypeSelector selector)
    {
        var registrationTypes = Enum.GetValues<RegistrationType>().Cast<RegistrationType>();

        var attributeLifetimes = new Dictionary<Type, ServiceLifetime>
        {
            { typeof(SingletonDependencyAttribute), ServiceLifetime.Singleton},
            { typeof(ScopedDependencyAttribute), ServiceLifetime.Scoped},
            { typeof(TransientDependencyAttribute), ServiceLifetime.Transient},
        };

        foreach (var registrationType in registrationTypes)
        {
            foreach (var attributeLifetime in attributeLifetimes)
            {
                selector
                    .AddClasses(cls => cls.Where(type => type.GetCustomAttribute(attributeLifetime.Key) is DependencyAttribute attribute && attribute.RegistrationType == registrationType))
                    .Register(registrationType, attributeLifetime.Value);
            }
        }

        return selector;
    }
}
