namespace Dddify.Domain;

/// <summary>
/// Represents an aggregate root. It's primary key may not be "Id" or it may have a composite primary key.
/// </summary>
public interface IAggregateRoot : IEntity
{
}

/// <summary>
/// Represents an aggregate root with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IAggregateRoot<TKey> : IEntity<TKey>, IAggregateRoot
{
}