namespace Dddify.SharedKernel;

/// <summary>
/// Represents the base type for entities whose identity may consist of multiple key values.
/// </summary>
public abstract class Entity : IEntity
{
    /// <summary>
    /// Returns the ordered key values that identify the entity.
    /// </summary>
    /// <returns>The ordered key values for the current entity.</returns>
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }
}

/// <summary>
/// Represents the base type for entities with a single strongly typed identifier.
/// </summary>
/// <typeparam name="TKey">The type of the entity identifier.</typeparam>
public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    private int? _requestedHashCode;

    /// <summary>
    /// Gets or sets the entity identifier.
    /// </summary>
    public virtual TKey Id { get; set; } = default!;

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id!.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }
        else
        {
            return base.GetHashCode();
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        return obj is Entity<TKey> entity && entity.IsTransient() && !IsTransient() && entity.Id!.Equals(Id);
    }

    /// <summary>
    /// Returns the ordered key values that identify the entity.
    /// </summary>
    /// <returns>An array containing the <see cref="Id"/> value.</returns>
    public override object[] GetKeys()
    {
        return [Id!];
    }

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Id = {Id}";
    }

    /// <summary>
    /// Determines whether the entity has not been assigned a persistent identifier yet.
    /// </summary>
    /// <returns><see langword="true"/> if the entity is transient; otherwise, <see langword="false"/>.</returns>
    public bool IsTransient()
    {
        return EqualityComparer<TKey>.Default.Equals(Id, default);
    }

    public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
    {
        return Equals(left, null) ? Equals(right, null) : left.Equals(right);
    }

    public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
    {
        return !(left == right);
    }
}
