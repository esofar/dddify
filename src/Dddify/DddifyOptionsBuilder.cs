using Dddify.EntityFrameworkCore;
using Dddify.Localization;
using Dddify.ResultWrapping;
using Dddify.Timing;
using Dddify.Users;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Dddify;

public class DddifyOptionsBuilder
{
    private readonly DddifyOptions _options;

    public DddifyOptionsBuilder() : this(new DddifyOptions())
    {
    }

    protected DddifyOptionsBuilder(DddifyOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    internal DddifyOptions Options => _options;

    private DddifyOptionsBuilder WithExtension<T>(T extension)
        where T : IOptionsExtension
    {
        _options.AddOrUpdateExtension(extension);
        return this;
    }

    /// <summary>
    /// Disables the unit of work pipeline behavior for command handling.
    /// </summary>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder DisableUnitOfWorkBehavior()
    {
        _options.UnitOfWorkBehaviorEnabled = false;
        return this;
    }

    /// <summary>
    /// Disables the validation pipeline behavior.
    /// </summary>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder DisableValidationBehavior()
    {
        _options.ValidationBehaviorEnabled = false;
        return this;
    }

    /// <summary>
    /// Configures MediatR registration used by Dddify.
    /// </summary>
    /// <param name="configure">The action used to configure <see cref="MediatRServiceConfiguration"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder ConfigureMediatR(Action<MediatRServiceConfiguration> configure)
    {
        _options.ConfigureMediatR = configure;
        return this;
    }

    /// <summary>
    /// Configures Scrutor-based service scanning used by Dddify.
    /// </summary>
    /// <param name="configure">The action used to configure the <see cref="ITypeSourceSelector"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder ConfigureScrutor(Action<ITypeSourceSelector> configure)
    {
        _options.ConfigureScrutor = configure;
        return this;
    }

    /// <summary>
    /// Configures the Mapster <see cref="TypeAdapterConfig"/> used by Dddify.
    /// </summary>
    /// <param name="configure">The action used to customize the Mapster type-adapter configuration.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder ConfigureMapster(Action<TypeAdapterConfig> configure)
    {
        _options.ConfigureMapster = configure;
        return this;
    }

    /// <summary>
    /// Sets the assemblies used by Dddify for convention-based registration and component discovery.
    /// </summary>
    /// <param name="assemblies">
    /// The assemblies scanned for Mapster configuration, validators, MediatR handlers, and Scrutor-based services.
    /// </param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder ScanAssemblies(params Assembly[] assemblies)
    {
        _options.Assemblies = assemblies;
        return this;
    }

    /// <summary>
    /// Registers timing services using the default Dddify implementations.
    /// </summary>
    /// <param name="configure">An optional action used to configure <see cref="TimingOptions"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddTiming(Action<TimingOptions>? configure = null)
        => WithExtension(new TimingOptionsExtension(configure));

    /// <summary>
    /// Registers Dddify's claims-based current-user services.
    /// </summary>
    /// <param name="configure">
    /// An optional action used to configure <see cref="CurrentUserOptions"/>, such as a custom
    /// user-id claim type, provider, or additional current-user service.
    /// </param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddCurrentUser(Action<CurrentUserOptions>? configure = null)
        => WithExtension(new CurrentUserOptionsExtension(configure));

    /// <summary>
    /// Registers JSON-based localization using the specified configuration section path.
    /// </summary>
    /// <param name="configSectionPath">The configuration section path used to bind JSON localization options.</param>
    /// <param name="configure">An optional action used to configure <see cref="JsonLocalizationOptions"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddLocalization(string configSectionPath, Action<JsonLocalizationOptions>? configure = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(configSectionPath);

        return WithExtension(new JsonLocalizationOptionsExtension(configSectionPath, configure));
    }

    /// <summary>
    /// Registers JSON-based localization using the default <c>Localization</c> configuration section.
    /// </summary>
    /// <param name="configure">An optional action used to configure <see cref="JsonLocalizationOptions"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddLocalization(Action<JsonLocalizationOptions>? configure = null)
        => AddLocalization("Localization", configure);

    /// <summary>
    /// Registers standardized API result output support.
    /// </summary>
    /// <param name="configure">An optional action used to configure <see cref="ApiResultWrappingOptions"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddApiResultWrapping(Action<ApiResultWrappingOptions>? configure = null)
        => WithExtension(new ApiResultWrappingExtension(configure));

    /// <summary>
    /// Registers the specified <see cref="DbContext"/> and enables Dddify unit of work integration for it.
    /// </summary>
    /// <typeparam name="TContextService">The service type used to resolve the context from the container.</typeparam>
    /// <typeparam name="TContextImplementation">The concrete <see cref="DbContext"/> implementation type.</typeparam>
    /// <param name="optionsAction">An optional action used to configure the <see cref="DbContextOptionsBuilder"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddDbContextWithUnitOfWork<TContextService, TContextImplementation>(Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContextImplementation : DbContext, TContextService
        => WithExtension(new DbContextUnitOfWorkOptionsExtension<TContextService, TContextImplementation>(optionsAction));

    /// <summary>
    /// Registers the specified <see cref="DbContext"/> and enables Dddify unit of work integration for it.
    /// </summary>
    /// <typeparam name="TContext">The <see cref="DbContext"/> type to register.</typeparam>
    /// <param name="optionsAction">An optional action used to configure the <see cref="DbContextOptionsBuilder"/>.</param>
    /// <returns>The current <see cref="DddifyOptionsBuilder"/> instance for chaining.</returns>
    public DddifyOptionsBuilder AddDbContextWithUnitOfWork<TContext>(Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
        => AddDbContextWithUnitOfWork<TContext, TContext>(optionsAction);
}
