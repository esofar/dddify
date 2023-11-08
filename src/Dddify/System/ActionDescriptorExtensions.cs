using Microsoft.AspNetCore.Mvc.Abstractions;

namespace Dddify.System;

/// <summary>
/// This class is used to provide <see cref="ActionDescriptor"/> type extension method.
/// </summary>
public static class ActionDescriptorExtensions
{
    public static bool HasAttrbute<T>(this ActionDescriptor target)
        where T : Attribute
    {
        return target.EndpointMetadata.Any(c => c.GetType() == typeof(T));
    }
}