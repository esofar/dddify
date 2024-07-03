using Dddify.Localization;
using Microsoft.Extensions.Logging;

namespace Dddify.Exceptions;

/// <summary>
/// Abstract base class representing a business exception.
/// </summary>
public abstract class BussinessException : Exception
{
    /// <summary>
    /// Gets or sets the localized name of the exception.
    /// </summary>
    public virtual string Name { get; } = string.Empty;

    /// <summary>
    /// Gets or sets the localized arguments associated with the exception.
    /// </summary>
    public virtual string[] Arguments { get; } = [];

    /// <summary>
    /// Gets or sets the type of the localized resource associated with the exception.
    /// </summary>
    /// <remarks>
    /// By default, it is set to <see cref="SharedResource"/> type.
    /// </remarks>
    public virtual Type ResourceType { get; } = typeof(SharedResource);

    /// <summary>
    /// Gets or sets the log level.
    /// </summary>
    /// <remarks>
    /// By default, it is set to <see cref="LogLevel.Warning"/>.
    /// </remarks>
    public virtual LogLevel LogLevel { get; set; } = LogLevel.Warning;

    /// <summary>
    /// Initializes a new instance of the <see cref="BussinessException"/> class with the specified error message.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    public BussinessException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BussinessException"/> class.
    /// </summary>
    public BussinessException()
    {
    }
}
