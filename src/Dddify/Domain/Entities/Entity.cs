namespace Dddify.Domain.Entities;

public abstract class Entity : IEntity
{
    public abstract object[] GetKeys();

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
    }
}

public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    private int? _requestedHashCode;

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

    public override object[] GetKeys()
    {
        return new object[] { Id! };
    }

    public override string ToString()
    {
        return $"[Entity: {GetType().Name}] Id = {Id}";
    }

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