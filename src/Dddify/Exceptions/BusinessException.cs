using Microsoft.Extensions.Logging;

namespace Dddify.Exceptions;

/// <summary>
/// Represents a business exception with a stable error contract and structured metadata.
/// </summary>
public abstract class BusinessException : Exception
{
    private readonly Dictionary<string, object?> _metadata = new(StringComparer.Ordinal);

    /// <summary>
    /// Gets the logical category of the exception for diagnostics and observability.
    /// </summary>
    public virtual string Category => "Business";

    /// <summary>
    /// Gets the stable error code used for API responses and localization lookup.
    /// </summary>
    public string? ErrorCode { get; private set; }

    /// <summary>
    /// Gets the formatting arguments associated with the error code.
    /// </summary>
    public object[] ErrorArgs { get; private set; } = [];

    /// <summary>
    /// Gets the severity used when logging this exception.
    /// </summary>
    public LogLevel LogLevel { get; private set; } = LogLevel.Warning;

    /// <summary>
    /// Gets the structured metadata that should flow into logs and diagnostics.
    /// </summary>
    public IReadOnlyDictionary<string, object?> Metadata => _metadata;

    /// <summary>
    /// Sets the stable error code for this exception.
    /// </summary>
    public void WithErrorCode(string errorCode)
    {
        ErrorCode = errorCode;
        ErrorArgs = [];
    }

    /// <summary>
    /// Sets the stable error code and the formatting arguments used for localization.
    /// </summary>
    public void WithErrorCode(string errorCode, params object[] args)
    {
        ErrorCode = errorCode;
        ErrorArgs = args ?? [];
    }

    /// <summary>
    /// Sets the severity used when logging this exception.
    /// </summary>
    public void WithLogLevel(LogLevel logLevel)
    {
        LogLevel = logLevel;
    }

    /// <summary>
    /// Adds a structured metadata property for this exception.
    /// </summary>
    public void WithMetadata(string key, object? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        _metadata[key] = value;
    }

    /// <summary>
    /// Adds multiple structured metadata properties for this exception.
    /// </summary>
    public void WithMetadata(IEnumerable<KeyValuePair<string, object?>> properties)
    {
        foreach (var property in properties)
        {
            WithMetadata(property.Key, property.Value);
        }
    }
}
