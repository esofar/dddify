namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Represents the API-facing projection of an exception after mapping.
/// </summary>
public sealed class ApiExceptionMapping
{
    /// <summary>
    /// Gets the HTTP status code that should be returned.
    /// </summary>
    public int StatusCode { get; init; }

    /// <summary>
    /// Gets the stable error code that should be returned.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets the localized or fallback error message that should be returned.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Gets field-level errors for validation failures.
    /// </summary>
    public IDictionary<string, string[]>? Errors { get; init; }

    /// <summary>
    /// Gets structured metadata that should be attached to logs or diagnostics.
    /// </summary>
    public IReadOnlyDictionary<string, object?>? Metadata { get; init; }
}
