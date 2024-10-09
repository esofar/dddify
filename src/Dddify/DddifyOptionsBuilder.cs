using Dddify.AspNetCore.Results;
using Dddify.Guids;
using Dddify.Localization;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Dddify;

public class DddifyOptionsBuilder
{
    protected readonly DddifyOptions _options;

    public DddifyOptionsBuilder()
    {
        _options = new DddifyOptions();
    }

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

    /// <summary>
    /// Adds the JSON localization extension.
    /// </summary>
    /// <param name="configure">An optional action to configure the JSON localization options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseJsonLocalization(Action<JsonLocalizationOptions>? configure = null)
    {
        return WithExtension(new JsonLocalizationOptionsExtension(configure));
    }

    /// <summary>
    /// Adds the JSON localization extension using the specified configuration section path.
    /// </summary>
    /// <param name="configSectionPath">The configuration section path for JSON localization.</param>
    /// <param name="configure">An optional action to configure the JSON localization options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseJsonLocalization(string configSectionPath, Action<JsonLocalizationOptions>? configure = null)
    {
        return WithExtension(new JsonLocalizationOptionsExtension(configSectionPath, configure));
    }

    /// <summary>
    /// Adds the API result wrapper extension.
    /// </summary>
    /// <param name="configure">An optional action to configure the API result wrapper options.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder UseApiResultWrapper(Action<ApiResultWrapperOptions>? configure = null)
    {
        return WithExtension(new ApiResultWrapperOptionsExtension(configure));
    }

    /// <summary>
    /// Adds or updates the specified <see cref="IOptionsExtension"/> to the options.
    /// </summary>
    /// <typeparam name="TExtension">The type of extension to be added or updated.</typeparam>
    /// <param name="extension">The extension to be added or updated.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    private DddifyOptionsBuilder WithExtension<TExtension>(TExtension extension)
        where TExtension : IOptionsExtension
    {
        _options.AddOrUpdateExtension(extension);
        return this;
    }

    /// <summary>
    /// Adds custom <see cref="IPipelineBehavior{TRequest, TResponse}"/> to the options.
    /// </summary>
    /// <param name="behaviors">The behaviors to be added.</param>
    /// <returns>The current instance of <see cref="DddifyOptionsBuilder"/>.</returns>
    public DddifyOptionsBuilder AddBehaviors(params Type[] behaviors)
    {
        _options.OpenBehaviors.AddRange(behaviors);
        return this;
    }

    public DddifyOptions Build()
    {
        return _options;
    }

}