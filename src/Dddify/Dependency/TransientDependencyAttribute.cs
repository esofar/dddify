namespace Dddify.Dependency;

/// <summary>
/// Indicates that a class should be registered as a transient service in the dependency injection container.
/// </summary>
/// <remarks>
/// Transient services are created each time they are requested from the dependency injection container.
/// </remarks>
/// <param name="registrationMode">
/// Specifies the registration mode used to determine how the service is registered
/// </param>
public class TransientDependencyAttribute(RegistrationMode registrationMode)
    : DependencyAttribute(registrationMode)
{
}