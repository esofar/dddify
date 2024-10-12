namespace Dddify.Domain;

/// <summary>
/// Represents an entity. It's primary key may not be "Id" or it may have a composite primary key.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Returns an array of ordered keys for this entity.
    /// </summary>
    /// <returns></returns>
    object[] GetKeys();
}

/// <summary>
/// Represents an entity with a single primary key with "Id" property.
/// </summary>
/// <typeparam name="TKey">Type of the primary key.</typeparam>
public interface IEntity<TKey> : IEntity, IHasKey<TKey>
{
}