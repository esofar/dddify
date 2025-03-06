using Dddify.AspNetCore.Results;
using Dddify.Guids;
using Dddify.Localization;
using Dddify.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Dddify;

public class DddifyOptionsBuilder
{
    protected DddifyOptions _options;

    public DddifyOptionsBuilder()
    {
        _options = new DddifyOptions();
    }

    public DddifyOptions Options => _options;

    /// <summary>
    /// Sets the <see cref="DateTimeKind"/> for date and time values.
    /// </summary>
    /// <param name="dateTimeKind">The <see cref="DateTimeKind"/> to be used.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder WithDateTimeKind(DateTimeKind dateTimeKind)
    {
        _options.DateTimeKind = dateTimeKind;
        return this;
    }

    /// <summary>
    /// Sets the type of sequential GUID to be used.
    /// </summary>
    /// <param name="sequentialGuidType">The type of sequential GUID.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder WithSequentialGuidType(SequentialGuidType sequentialGuidType)
    {
        _options.SequentialGuidType = sequentialGuidType;
        return this;
    }

    public DddifyOptionsBuilder EnableUnitOfWorkBehavior(bool enabled = true)
    {
        _options.UnitOfWorkBehaviorEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Enables or disables the validation behavior.
    /// </summary>
    /// <param name="enabled">A boolean value indicating whether the validation behavior should be enabled.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder EnableValidationBehaviour(bool enabled = true)
    {
        _options.ValidationBehaviourEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Adds the current user service.
    /// </summary>
    /// <typeparam name="TCurrentUserImplementation">The type of the current user service.</typeparam>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder AddCurrentUser<TCurrentUserImplementation>()
        where TCurrentUserImplementation : class, ICurrentUser
        => WithExtension(new CurrentUserOptionsExtension<TCurrentUserImplementation>());

    /// <summary>
    /// Adds the current user service with extended user service and implementation.
    /// </summary>
    /// <typeparam name="TCurrentUserImplementation">The type of the current user service implementation.</typeparam>
    /// <typeparam name="TExtendedCurrentUserService">The type of the extended current user service.</typeparam>
    /// <typeparam name="TExtendedCurrentUserImplementation">The type of the extended current user service implementation.</typeparam>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder AddCurrentUser<TCurrentUserImplementation, TExtendedCurrentUserService, TExtendedCurrentUserImplementation>()
       where TCurrentUserImplementation : class, ICurrentUser
       where TExtendedCurrentUserService : class, ICurrentUser
       where TExtendedCurrentUserImplementation : class, ICurrentUser, TExtendedCurrentUserService
       => WithExtension(new CurrentUserOptionsExtension<TCurrentUserImplementation>(typeof(TExtendedCurrentUserService), typeof(TExtendedCurrentUserImplementation)));

    /// <summary>
    /// Adds the JSON localization using the specified configuration section path.
    /// </summary>
    /// <param name="configSectionPath">The configuration section path for JSON localization.</param>
    /// <param name="configure">An optional action to configure the JSON localization options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseJsonLocalization(string configSectionPath, Action<JsonLocalizationOptions>? configure = null)
       => WithExtension(new JsonLocalizationOptionsExtension(configSectionPath, configure));

    /// <summary>
    /// Adds the JSON localization using `JsonLocalization` configuration section path.
    /// </summary>
    /// <param name="configure">An optional action to configure the JSON localization options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseJsonLocalization(Action<JsonLocalizationOptions>? configure = null)
        => UseJsonLocalization("JsonLocalization", configure);

    /// <summary>
    /// Adds the API result wrapper extension.
    /// </summary>
    /// <param name="configure">An optional action to configure the API result wrapper options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseApiResultWrapper(Action<ApiResultWrapperOptions>? configure = null)
        => WithExtension(new ApiResultWrapperOptionsExtension(configure));

    /// <summary>
    /// Registers the given context.
    /// </summary>
    /// <typeparam name="TContextService">The class or interface that will be used to resolve the context from the container.</typeparam>
    /// <typeparam name="TContextImplementation">The concrete implementation type to create.</typeparam>
    /// <param name="optionsAction">An optional action to configure the <see cref="DbContextOptions" /> for the context.</param>
    /// <returns></returns>
    public DddifyOptionsBuilder AddDbContext<TContextService, TContextImplementation>(Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContextImplementation : DbContext, TContextService
        => WithExtension(new DbContextOptionsExtension<TContextService, TContextImplementation>(optionsAction));

    /// <summary>
    /// Registers the given context.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be registered.</typeparam>
    /// <param name="optionsAction">An optional action to configure the <see cref="DbContextOptions" /> for the context.</param>
    /// <returns></returns>
    public DddifyOptionsBuilder AddDbContext<TContext>(Action<DbContextOptionsBuilder>? optionsAction = null)
        where TContext : DbContext
        => AddDbContext<TContext, TContext>(optionsAction);

    private DddifyOptionsBuilder WithExtension<T>(T extension)
        where T : IOptionsExtension
    {
        _options.AddOrUpdateExtension(extension);
        return this;
    }

    /// <summary>
    /// Configures MediatR services.
    /// </summary>
    /// <param name="configure">An action to configure MediatR services.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder CustomiseMediatR(Action<MediatRServiceConfiguration> configure)
    {
        _options.ConfigureMediatr = configure;
        return this;
    }

    /// <summary>
    /// Scans the project assemblies and register services using Scrutor.
    /// </summary>
    /// <param name="configure">An action to configure the implementation type selector.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder ScanFromProjectAssemblies(Action<IImplementationTypeSelector> configure)
    {
        _options.ConfigureScrutor = configure;
        return this;
    }
}