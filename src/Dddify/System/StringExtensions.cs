namespace System;

/// <summary>
/// This class is used to provide <see cref="string"/> type extension method.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Splits the current string into substrings based on the string in an array.
    /// </summary>
    /// <param name="target">The string to split.</param>
    /// <param name="separator">The separator string, default is comma.</param>
    /// <returns>An array whose elements contain the substrings in this string that are delimited by a separator.</returns>
    public static string[] ToArray(this string target, string separator = ",")
    {
        return target.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);
    }

    /// <summary>
    /// Converts a string to <see cref="Guid"/>.
    /// </summary>
    /// <param name="target">The string to convert.</param>
    /// <returns>The converted GUID. Returns <see cref="Guid.Empty"/> if convert failed.</returns>
    public static Guid ToGuid(this string target)
    {
        return Guid.TryParse(target, out var guid) ? guid : Guid.Empty;
    }

    /// <summary>
    /// Converts a string representation of an enum value to the corresponding enum value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="target">The string representation of the enum value.</param>
    /// <param name="ignoreCase">Indicates whether the conversion should be case-insensitive. The default is <c>true</c>.</param>
    /// <returns>The enum value that corresponds to the input string.</returns>
    /// <exception cref="InvalidCastException">Thrown if the string cannot be converted to <typeparamref name="TEnum"/>.</exception>
    public static TEnum ToEnum<TEnum>(this string target, bool ignoreCase = true)
        where TEnum : struct, Enum
    {
        if (Enum.TryParse(target, ignoreCase, out TEnum result))
        {
            return result;
        }

        throw new InvalidCastException($"Cannot convert {target} to {typeof(TEnum).Name}.");
    }
}