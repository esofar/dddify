namespace Dddify.Dependency;

/// <summary>
/// Base attribute used to mark a class for automatic registration in the dependency injection container.
/// </summary>
/// <remarks>
/// This attribute allows specifying how the annotated type should be registered, including how it is exposed (e.g., as itself or via interfaces).
/// </remarks>
/// <remarks>
/// Initializes a new instance of the <see cref="DependencyAttribute"/> class with the specified registration mode.
/// </remarks>
/// <param name="registrationMode">
/// The registration mode that defines how the annotated type will be registered in the dependency injection container.
/// </param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class DependencyAttribute(RegistrationMode registrationMode) : Attribute
{
    /// <summary>
    /// Gets the registration mode that determines how the service is registered and exposed.
    /// </summary>
    public RegistrationMode RegistrationMode { get; } = registrationMode;
}