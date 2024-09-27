namespace Dddify.Dependency;

/// <summary>
/// Indicates that a class will be registered to the <see cref="ServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime.
/// </summary>
public class ScopedDependencyAttribute : DependencyAttribute
{
    /// <inheritdoc/>
    public ScopedDependencyAttribute()
        : base(RegistrationType.AsImplementedInterfaces)
    {
    }

    /// <inheritdoc/>
    public ScopedDependencyAttribute(RegistrationType registrationType)
        : base(registrationType)
    {
    }
}