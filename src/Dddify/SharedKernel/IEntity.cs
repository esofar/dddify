namespace Dddify.SharedKernel;

/// <summary>
/// Represents an entity in the domain model.
/// </summary>
/// <remarks>
/// This non-generic contract is intended for entities whose identity is not modeled as a single
/// strongly typed <c>Id</c> property or whose identity consists of multiple key values.
/// </remarks>
public interface IEntity
{
    /// <summary>
    /// Returns the ordered key values that uniquely identify the entity.
    /// </summary>
    /// <returns>
    /// An array containing the key values in their defined order.
    /// </returns>
    object[] GetKeys();
}

/// <summary>
/// Represents an entity with a strongly typed single-key identity.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public interface IEntity<TKey> : IEntity
{
}
