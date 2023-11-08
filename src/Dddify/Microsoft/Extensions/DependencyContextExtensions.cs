using System.Reflection;

namespace Microsoft.Extensions.DependencyModel;

public static class DependencyContextExtensions
{
    public static IEnumerable<Assembly> GetInternalAssemblies(this DependencyContext context)
    {
        foreach (var library in context.CompileLibraries)
        {
            if (library.Type == "project" || (library.Type == "package" && library.Name.Contains("dddify", StringComparison.InvariantCultureIgnoreCase)))
            {
                yield return Assembly.Load(library.Name);
            }
        }
    }
}
