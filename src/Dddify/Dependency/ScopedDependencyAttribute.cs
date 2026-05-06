namespace Dddify.Dependency;

/// <summary>
/// Indicates that a class should be registered as a scoped service in the dependency injection container.
/// </summary>
/// <remarks>
/// Scoped services are created once per scope. In ASP.NET Core applications, a scope typically corresponds to a single HTTP request.
/// </remarks>
/// <param name="registrationMode">
/// Specifies the registration mode used to determine how the service is registered
/// </param>
public class ScopedDependencyAttribute(RegistrationMode registrationMode)
    : DependencyAttribute(registrationMode)
{
}