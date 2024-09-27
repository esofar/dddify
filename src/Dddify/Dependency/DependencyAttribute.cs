namespace Dddify.Dependency;

/// <summary>
/// Represents base class for dependency attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public abstract class DependencyAttribute : Attribute
{
    /// <summary>
    /// The registration type for the dependency.
    /// </summary>
    public RegistrationType RegistrationType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyAttribute"/> class. The default registration type is <see cref="RegistrationType.AsImplementedInterfaces"/>.
    /// </summary>
    protected DependencyAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyAttribute"/> class.
    /// </summary>
    /// <param name="registrationType">The registration type for the dependency.</param>
    protected DependencyAttribute(RegistrationType registrationType) => RegistrationType = registrationType;
}