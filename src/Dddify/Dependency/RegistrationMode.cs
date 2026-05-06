namespace Dddify.Dependency;

/// <summary>
/// Defines how a service is registered and exposed in the dependency injection container.
/// </summary>
public enum RegistrationMode
{
    /// <summary>
    /// Registers the service using its concrete implementation type.
    /// </summary>
    AsSelf,

    /// <summary>
    /// Registers the service using a convention-based matching interface.
    /// </summary>
    /// <remarks>
    /// For example, a class named <c>MyService</c> will be registered as <c>IMyService</c>.
    /// </remarks>
    AsMatchingInterface,

    /// <summary>
    /// Registers the service as all interfaces implemented by the type.
    /// </summary>
    AsImplementedInterfaces,
}