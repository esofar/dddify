using Dddify.Messaging.Behaviors;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Indicates that the decorated request should bypass the unit of work pipeline behavior.
/// </summary>
/// <remarks>
/// Apply this attribute to command types that must not start, join, commit, or roll back a unit of work
/// within <see cref="UnitOfWorkBehavior{TRequest, TResponse}"/>. This is useful for commands that do not
/// modify persistent state or that manage transactions explicitly in a different scope.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public sealed class SkipUnitOfWorkBehaviorAttribute : Attribute
{
}