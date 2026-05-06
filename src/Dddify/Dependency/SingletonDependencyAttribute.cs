namespace Dddify.Dependency;

/// <summary>
/// Indicates that a class should be registered as a singleton service in the dependency injection container.
/// </summary>
/// <remarks>
/// Singleton services are created once and shared for the entire lifetime of the application.
/// </remarks>
/// <param name="registrationMode">
/// Specifies the registration mode used to determine how the service is registered
/// </param>
public class SingletonDependencyAttribute(RegistrationMode registrationMode)
    : DependencyAttribute(registrationMode)
{
}