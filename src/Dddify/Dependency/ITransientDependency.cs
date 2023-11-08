using Microsoft.Extensions.DependencyInjection;

namespace Dddify.Dependency;

/// <summary>
/// Represents a transient dependency. Classes implementing this interface will be registered to the <see cref="ServiceCollection"/> with <see cref="ServiceLifetime.Transient"/> lifetime.
/// </summary>
public interface ITransientDependency
{
}