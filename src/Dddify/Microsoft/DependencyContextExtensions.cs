using System.Reflection;

namespace Microsoft.Extensions.DependencyModel;

public static class DependencyContextExtensions
{
    /// <summary>
    /// Retrieves assemblies of project.
    /// </summary>
    /// <param name="context">The dependency context containing information about all compiled libraries.</param>
    /// <returns>Returns an array containing the specified project and NuGet package assemblies.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Assembly[] GetProjectAssemblies(this DependencyContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.CompileLibraries
            .Where(library => library.Type == "project")
            .Select(library => Assembly.Load(library.Name))
            .ToArray();
    }
}
