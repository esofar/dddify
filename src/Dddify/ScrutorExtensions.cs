using Dddify.Dependency;
using Dddify.Domain;
using Scrutor;
using System.Reflection;

namespace Dddify;

public static class ScrutorExtensions
{
    public static IImplementationTypeSelector RegisterDependencyServices(this IImplementationTypeSelector selector)
    {
        var registrationTypes = Enum.GetValues(typeof(RegistrationType)).Cast<RegistrationType>();

        var attributeTypes = new List<Type>
        {
            typeof(SingletonDependencyAttribute),
            typeof(ScopedDependencyAttribute),
            typeof(TransientDependencyAttribute)
        };

        foreach (var registrationType in registrationTypes)
        {
            foreach (var attributeType in attributeTypes)
            {
                var classes = selector.AddClasses(cls => cls.Where(type => type.GetCustomAttribute(attributeType) is DependencyAttribute attribute && attribute.RegistrationType == registrationType));

                switch (registrationType)
                {
                    case RegistrationType.AsSelf:
                        classes.AsSelf().WithSingletonLifetime();
                        break;

                    case RegistrationType.AsMatchingInterface:
                        classes.AsMatchingInterface().WithSingletonLifetime();
                        break;

                    case RegistrationType.AsImplementedInterfaces:
                        classes.AsImplementedInterfaces().WithSingletonLifetime();
                        break;
                }
            }
        }

        return selector;
    }

    public static IImplementationTypeSelector RegisterDomainServices(this IImplementationTypeSelector selector)
    {
        return selector.AddClasses(classes => classes.AssignableTo<IDomainService>())
             .AsSelf()
             .WithTransientLifetime();
    }
}
