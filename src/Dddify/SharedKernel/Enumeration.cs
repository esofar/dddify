using System.Reflection;

namespace Dddify.SharedKernel;

/// <summary>
/// Represents the base type for the enumeration class pattern.
/// </summary>
/// <remarks>
/// Use this type when a domain concept behaves like an enumeration but requires richer semantics than
/// a primitive <see langword="enum"/>, such as behavior, validation, or persistence-friendly identifiers.
/// </remarks>
public abstract class Enumeration : IComparable
{
    /// <summary>
    /// Gets the display name of the enumeration item.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the persisted numeric value of the enumeration item.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Enumeration"/> class.
    /// </summary>
    /// <param name="id">The numeric value of the enumeration item.</param>
    /// <param name="name">The display name of the enumeration item.</param>
    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    /// <summary>
    /// Returns all declared items of the specified enumeration type.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <returns>All declared items of the specified enumeration type.</returns>
    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    /// <summary>
    /// Returns the absolute numeric difference between two enumeration items.
    /// </summary>
    /// <param name="firstValue">The first enumeration item.</param>
    /// <param name="secondValue">The second enumeration item.</param>
    /// <returns>The absolute difference between the numeric identifiers of the two items.</returns>
    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    /// <summary>
    /// Gets an enumeration item by its numeric value.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="value">The numeric value to match.</param>
    /// <returns>The matching enumeration item.</returns>
    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = Parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    /// <summary>
    /// Gets an enumeration item by its display name.
    /// </summary>
    /// <typeparam name="T">The enumeration type.</typeparam>
    /// <param name="displayName">The display name to match.</param>
    /// <returns>The matching enumeration item.</returns>
    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        var matchingItem = Parse<T, string>(displayName, "display name", item => item.Name == displayName);
        return matchingItem;
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        return matchingItem ?? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
    }

    /// <summary>
    /// Compares the current enumeration item with another enumeration item.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// A signed integer that indicates the relative order of the instances being compared.
    /// </returns>
    public int CompareTo(object? obj)
    {
        if (obj is null)
            return 1;

        if (obj is not Enumeration other)
            throw new ArgumentException($"Object must be of type {nameof(Enumeration)}.", nameof(obj));

        return Id.CompareTo(other.Id);
    }
}
