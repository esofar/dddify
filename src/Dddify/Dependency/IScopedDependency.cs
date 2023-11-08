using Microsoft.Extensions.DependencyInjection;

namespace Dddify.Dependency;

/// <summary>
/// Represents a scoped dependency. Classes implementing this interface will be registered to the <see cref="ServiceCollection"/> with <see cref="ServiceLifetime.Scoped"/> lifetime.
/// </summary>
public interface IScopedDependency
{
}