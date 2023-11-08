using System.Globalization;

namespace System;

/// <summary>
/// Extension methods for all objects.
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// Used to simplify and beautify casting an object to a type.
    /// </summary>
    /// <typeparam name="T">Type to be casted.</typeparam>
    /// <param name="target">Object to cast.</param>
    /// <returns>Casted object.</returns>
    public static T As<T>(this object target)
        where T : class
    {
        return (T)target;
    }

    /// <summary>
    /// Converts given object to a value type using <see cref="Convert.ChangeType(object,Type)"/> method.
    /// </summary>
    /// <param name="target">Object to be converted.</param>
    /// <typeparam name="T">Type of the target object.</typeparam>
    /// <returns>Converted object.</returns>
    public static T To<T>(this object target)
        where T : struct
    {
        return (T)Convert.ChangeType(target, typeof(T), CultureInfo.InvariantCulture);
    }
}