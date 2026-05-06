namespace Dddify.SharedKernel;

/// <summary>
/// Represents the base type for value objects in the domain model.
/// </summary>
/// <remarks>
/// Value objects are compared by the values of their components rather than by identity.
/// Derive from this type when a concept is defined entirely by its attributes and does not
/// require a stable identifier.
/// </remarks>
public abstract class ValueObject
{
    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The left value object.</param>
    /// <param name="right">The right value object.</param>
    /// <returns><see langword="true"/> if both instances are equal; otherwise, <see langword="false"/>.</returns>
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if (left is null ^ right is null)
        {
            return false;
        }

        return left?.Equals(right!) != false;
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The left value object.</param>
    /// <param name="right">The right value object.</param>
    /// <returns><see langword="true"/> if the instances are not equal; otherwise, <see langword="false"/>.</returns>
    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !EqualOperator(left, right);
    }

    /// <summary>
    /// Returns the ordered components that participate in equality comparison.
    /// </summary>
    /// <returns>The sequence of values that define the value object's equality.</returns>
    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x != null ? x.GetHashCode() : 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Creates a shallow copy of the current value object.
    /// </summary>
    /// <returns>A shallow copy of the current instance.</returns>
    public ValueObject? GetCopy()
    {
        return MemberwiseClone() as ValueObject;
    }
}
