namespace Dddify.Dependency;

/// <summary>
/// Indicates that a class will be registered to the <see cref="ServiceCollection"/> with <see cref="ServiceLifetime.Singleton"/> lifetime.
/// </summary>
public class SingletonDependencyAttribute : DependencyAttribute
{
    /// <inheritdoc/>
    public SingletonDependencyAttribute()
        : base(RegistrationType.AsImplementedInterfaces)
    {
    }

    /// <inheritdoc/>
    public SingletonDependencyAttribute(RegistrationType registrationType)
        : base(registrationType)
    {
    }
}