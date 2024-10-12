namespace Dddify.Domain;

/// <summary>
/// Represents an entity that has a key of type <see cref="IHasKey{TKey}"/>.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
public interface IHasKey<TKey>
{
    /// <summary>
    /// Gets or sets the identifier (key) of the entity.
    /// </summary>
    TKey Id { get; set; }
}