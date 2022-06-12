using Dddify.DependencyInjection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection;

public static class DddifyBuilderExtensions
{
    public static IDddifyBuilder AddApplication(this IDddifyBuilder builder, params Assembly[] assemblies)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assemblies);

        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper, ServiceMapper>();

        builder.Services.AddMediatR(assemblies);
        builder.Services.AddValidatorsFromAssemblies(assemblies);

        return builder;
    }

    public static IDddifyBuilder AddApplication(this IDddifyBuilder builder)
    {
        var entryAssembly = Assembly.GetEntryAssembly();
        var applicationAssemblies = new List<Assembly> { entryAssembly! };

        var applicationAssemblyNames = entryAssembly!
            .GetReferencedAssemblies()
            .Where(c => c.Name!.StartsWith(entryAssembly.GetFullNamePrefix(".")) && c.Name.EndsWith("Application"))
            .ToList();

        applicationAssemblies
            .AddRange(applicationAssemblyNames.LoadAssemblies());

        return builder.AddApplication(applicationAssemblies.ToArray());
    }
}
