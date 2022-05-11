using Dddify;

namespace System.Reflection;

/// <summary>
/// Extension methods for <see cref="Assembly"/>.
/// </summary>
public static class AssemblyExtensions
{
    public static string GetFullNamePrefix(this Assembly assembly, string nameSectionSeparator)
    {
        var fullname = assembly.FullName ?? string.Empty;
        return fullname[..(fullname.IndexOf(nameSectionSeparator, StringComparison.Ordinal) + 1)];
    }

    public static IEnumerable<Assembly> LoadAssemblies(this IEnumerable<AssemblyName> assemblyNames)
    {
        var assemblies = new List<Assembly>();

        foreach (var assemblyName in assemblyNames)
        {
            try
            {
                // Try to load the referenced assembly...
                assemblies.Add(Assembly.Load(assemblyName));
            }
            catch
            {
                // Failed to load assembly. Skip it.
            }
        }

        return assemblies;
    }
}
