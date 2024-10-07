namespace Dddify.Domain;

/// <summary>
/// Represents base class for <see cref="IAggregateRoot"/>.
/// </summary>
public abstract class AggregateRoot : Entity, IAggregateRoot
{
}

/// <summary>
/// Represents base class for <see cref="IAggregateRoot{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the primary key for the aggregate root.</typeparam>
public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
{
}