using Dddify.AspNetCore.Results;
using Dddify.Guids;
using Dddify.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace Dddify;

public static class DddifyOptionsBuilderExts
{
    public static DddifyOptionsBuilder WithDateTimeKind1(this DddifyOptionsBuilder builder, DateTimeKind dateTimeKind)
    {
        builder.Options.DateTimeKind = dateTimeKind;
        return builder;
    }
}

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

    public DddifyOptionsBuilder EnableValidationBehaviour(bool enabled = true)
    {
        _options.ValidationBehaviourEnabled = enabled;
        return this;
    }

    /// <summary>
    /// Adds the JSON localization extension.
    /// </summary>
    /// <param name="configure">An optional action to configure the JSON localization options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseJsonLocalization(Action<JsonLocalizationOptions>? configure = null)
        => WithExtension(new JsonLocalizationOptionsExtension(configure));

    /// <summary>
    /// Adds the JSON localization extension using the specified configuration section path.
    /// </summary>
    /// <param name="configSectionPath">The configuration section path for JSON localization.</param>
    /// <param name="configure">An optional action to configure the JSON localization options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseJsonLocalization(string configSectionPath, Action<JsonLocalizationOptions>? configure = null)
       => WithExtension(new JsonLocalizationOptionsExtension(configSectionPath, configure));

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
    public DddifyOptionsBuilder AddDbContext<TContextService, TContextImplementation>(
        Action<DbContextOptionsBuilder>? optionsAction = null) where TContextImplementation : DbContext, TContextService
        => WithExtension(new DbContextOptionsExtension<TContextService, TContextImplementation>(optionsAction));

    /// <summary>
    /// Registers the given context.
    /// </summary>
    /// <typeparam name="TContext">The type of context to be registered.</typeparam>
    /// <param name="optionsAction">An optional action to configure the <see cref="DbContextOptions" /> for the context.</param>
    /// <returns></returns>
    public DddifyOptionsBuilder AddDbContext<TContext>(
        Action<DbContextOptionsBuilder>? optionsAction = null) where TContext : DbContext
        => AddDbContext<TContext, TContext>(optionsAction);

    /// <summary>
    /// Adds or updates the specified <see cref="IOptionsExtension"/> to the options.
    /// </summary>
    /// <typeparam name="TExtension">The type of extension to be added or updated.</typeparam>
    /// <param name="extension">The extension to be added or updated.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    private DddifyOptionsBuilder WithExtension<TExtension>(TExtension extension)
        where TExtension : IOptionsExtension
    {
        Options.AddOrUpdateExtension(extension);
        return this;
    }

    public DddifyOptionsBuilder ConfigureMediatR(Action<MediatRServiceConfiguration> configure)
    {
        _options.MediatrOptions = configure;
        return this;
    }

    public DddifyOptionsBuilder ConfigureScrutor(Action<ITypeSourceSelector> configure)
    {
        _options.ScrutorOptions = configure;
        return this;
    }
}