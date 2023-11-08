namespace System;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether an instance of this type can be assigned to an instance of the <typeparamref name="TTarget"></typeparamref>.
    /// Internally uses <see cref="Type.IsAssignableFrom"/>.
    /// </summary>
    /// <typeparam name="TTarget">The type to compare with the current type.</typeparam>
    /// <param name="type">The current type.</param>
    /// <returns></returns>
    public static bool IsAssignableTo<TTarget>(this Type type)
    {
        return type.IsAssignableTo(typeof(TTarget));
    }

    /// <summary>
    /// Determines whether the current type implements/inherits the specified <paramref name="genericType"></paramref>.
    /// </summary>
    /// <param name="type">The current type.</param>
    /// <param name="genericType">The generic type.</param>
    /// <returns></returns>
    public static bool IsAssignableToGenericType(this Type type, Type genericType)
    {
        return type.IsGenericType && !type.IsAbstract && type.GetInterface(genericType.Name) != null;
    }
}