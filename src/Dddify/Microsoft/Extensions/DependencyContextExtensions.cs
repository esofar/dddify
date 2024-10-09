using System.Reflection;

namespace Microsoft.Extensions.DependencyModel;

public static class DependencyContextExtensions
{
    /// <summary>
    /// Retrieves assemblies of project and specific NuGet packages (with names containing "dddify").
    /// </summary>
    /// <param name="context">The dependency context containing information about all compiled libraries.</param>
    /// <returns>Returns an array containing the specified project and NuGet package assemblies.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Assembly[] GetProjectAndDddifyAssemblies(this DependencyContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        return context.CompileLibraries
            .Where(library => library.Type == "project" || (library.Type == "nuget" && library.Name.Contains("dddify")))
            .Select(library => Assembly.Load(library.Name))
            .ToArray();
    }
}
