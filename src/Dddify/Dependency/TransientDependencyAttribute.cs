namespace Dddify.Dependency;

/// <summary>
/// Indicates that a class will be registered to the <see cref="ServiceCollection"/> with <see cref="ServiceLifetime.Transient"/> lifetime.
/// </summary>
public class TransientDependencyAttribute : DependencyAttribute
{
    /// <inheritdoc/>
    public TransientDependencyAttribute()
        : base(RegistrationType.AsImplementedInterfaces)
    {
    }

    /// <inheritdoc/>
    public TransientDependencyAttribute(RegistrationType registrationType)
        : base(registrationType)
    {
    }
}