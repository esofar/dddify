using Microsoft.Extensions.DependencyInjection;

namespace Dddify.Dependency;

/// <summary>
/// Represents a singleton dependency. Classes implementing this interface will be registered to the <see cref="ServiceCollection"/> with <see cref="ServiceLifetime.Singleton"/> lifetime.
/// </summary>
public interface ISingletonDependency
{
}