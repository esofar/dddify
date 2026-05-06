using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Collections.Concurrent;
using System.Reflection;

namespace Dddify;

/// <summary>
/// Represents the internal options collected while configuring Dddify service registration.
/// </summary>
public partial class DddifyOptions
{
    private readonly ConcurrentDictionary<Type, IOptionsExtension> _extensionsMap;

    /// <summary>
    /// Initializes a new instance of the <see cref="DddifyOptions"/> class.
    /// </summary>
    public DddifyOptions()
    {
        _extensionsMap = new ConcurrentDictionary<Type, IOptionsExtension>();
    }

    /// <summary>
    /// Gets or sets a value indicating whether the validation pipeline behavior should be registered.
    /// </summary>
    internal bool ValidationBehaviorEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the unit of work pipeline behavior should be registered.
    /// </summary>
    internal bool UnitOfWorkBehaviorEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets an optional callback for configuring MediatR registration.
    /// </summary>
    internal Action<MediatRServiceConfiguration>? ConfigureMediatR { get; set; }

    /// <summary>
    /// Gets or sets an optional callback for configuring Scrutor scanning rules.
    /// </summary>
    internal Action<ITypeSourceSelector>? ConfigureScrutor { get; set; }

    /// <summary>
    /// Gets or sets an optional callback for configuring the Mapster type-adapter configuration.
    /// </summary>
    internal Action<TypeAdapterConfig>? ConfigureMapster { get; set; }

    /// <summary>
    /// Gets or sets the assemblies used for scanning validators, handlers, and convention-based services.
    /// </summary>
    internal Assembly[]? Assemblies { get; set; }

    /// <summary>
    /// Gets the registered Dddify options extensions.
    /// </summary>
    internal List<IOptionsExtension> Extensions => [.. _extensionsMap.Values];

    /// <summary>
    /// Adds or replaces an options extension of the specified type.
    /// </summary>
    /// <typeparam name="TExtension">The extension type.</typeparam>
    /// <param name="extension">The extension instance to add or update.</param>
    internal void AddOrUpdateExtension<TExtension>(TExtension extension)
        where TExtension : IOptionsExtension
    {
        ArgumentNullException.ThrowIfNull(extension);

        _extensionsMap.AddOrUpdate(
            typeof(TExtension),
            extension,
            (key, oldValue) => oldValue = extension);
    }
}
