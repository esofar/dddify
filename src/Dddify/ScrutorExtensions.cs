using Dddify.Dependency;
using Dddify.Domain;
using Dddify.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Dddify;

public static class ScrutorExtensions
{
    public static IImplementationTypeSelector RegisterAttributeDependencies(this IImplementationTypeSelector selector)
    {
        var registrationTypes = Enum.GetValues(typeof(RegistrationType)).Cast<RegistrationType>();

        var attributeLifetimes = new Dictionary<Type, ServiceLifetime>
        {
            { typeof(SingletonDependencyAttribute), ServiceLifetime.Singleton},
            { typeof(ScopedDependencyAttribute), ServiceLifetime.Scoped},
            { typeof(TransientDependencyAttribute), ServiceLifetime.Transient},
        };

        void WithLifetime(ILifetimeSelector lifetimeSelector, ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    lifetimeSelector.WithSingletonLifetime();
                    break;
                case ServiceLifetime.Scoped:
                    lifetimeSelector.WithScopedLifetime();
                    break;
                case ServiceLifetime.Transient:
                    lifetimeSelector.WithTransientLifetime();
                    break;
            }
        }

        void RegisterServices(IServiceTypeSelector serviceTypeSelector, RegistrationType registrationType, ServiceLifetime lifetime)
        {
            switch (registrationType)
            {
                case RegistrationType.AsSelf:
                    WithLifetime(serviceTypeSelector.AsSelf(), lifetime);
                    break;
                case RegistrationType.AsMatchingInterface:
                    WithLifetime(serviceTypeSelector.AsMatchingInterface(), lifetime);
                    break;
                case RegistrationType.AsImplementedInterfaces:
                    WithLifetime(serviceTypeSelector.AsImplementedInterfaces(), lifetime);
                    break;
            }
        }

        foreach (var registrationType in registrationTypes)
        {
            foreach (var attributeLifetime in attributeLifetimes)
            {
                var classes = selector.AddClasses(
                    cls => cls.Where(
                        type => type.GetCustomAttribute(attributeLifetime.Key) is DependencyAttribute attribute && attribute.RegistrationType == registrationType));

                RegisterServices(classes, registrationType, attributeLifetime.Value);
            }
        }

        return selector;
    }

    public static IImplementationTypeSelector RegisterDomainServices(this IImplementationTypeSelector selector)
    {
        return selector
            .AddClasses(classes => classes.AssignableTo<IDomainService>())
                .AsSelf()
                .WithTransientLifetime();
    }

    public static IImplementationTypeSelector RegisterRepositories(this IImplementationTypeSelector selector)
    {
        return selector
            .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)))
                .AsMatchingInterface()
                .WithScopedLifetime();
    }
}
