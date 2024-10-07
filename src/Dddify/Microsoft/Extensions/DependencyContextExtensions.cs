using System.Reflection;

namespace Microsoft.Extensions.DependencyModel;

public static class DependencyContextExtensions
{
    /// <summary>
    /// Retrieves an array of assemblies that represent the projects in the specified dependency context.
    /// This method filters the compile libraries to include only those of type "project",
    /// and loads each project assembly by its name.
    /// </summary>
    /// <param name="context">The dependency context containing information about the project's dependencies.</param>
    /// <returns>An array of assemblies corresponding to the project libraries.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="context"/> is null.</exception>
    public static Assembly[] GetProjectAssemblies(this DependencyContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.CompileLibraries
            .Where(library => library.Type == "project")
            .Select(library => Assembly.Load(library.Name))
            .ToArray();
    }
}
