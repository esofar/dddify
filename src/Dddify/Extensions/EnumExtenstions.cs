using System.Reflection;
using System.ComponentModel;

namespace System;

/// <summary>
/// This class is used to provide <see cref="Enum"/> type extension method.
/// </summary>
public static class EnumExtenstions
{
    public static string GetDescription(this Enum target)
    {
        var type = target.GetType();
        var fieldName = Enum.GetName(type, target);

        if (fieldName != null)
        {
            var attribute = type?.GetField(fieldName)?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? fieldName;
        }
        else
        {
            return string.Empty;
        }
    }
}
