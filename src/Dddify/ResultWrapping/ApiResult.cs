using System.Text.Json.Serialization;

namespace Dddify.ResultWrapping;

/// <summary>
/// Represents the standard response envelope returned by wrapped API endpoints.
/// </summary>
public class ApiResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the request completed successfully.
    /// </summary>
    [JsonPropertyOrder(0)]
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets the stable error code when the request fails.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the localized or fallback error message when the request fails.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the trace identifier that can be used for diagnostics.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TraceId { get; set; }
}

/// <summary>
/// Represents the standard response envelope returned by wrapped API endpoints with a data payload.
/// </summary>
/// <typeparam name="T">The payload type.</typeparam>
public class ApiResult<T> : ApiResult
{
    /// <summary>
    /// Gets or sets the response payload.
    /// </summary>
    [JsonPropertyOrder(1)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public T? Data { get; set; }
}

/// <summary>
/// Represents a failed API response envelope that also contains field-level errors.
/// </summary>
public class ApiResultWithErrors : ApiResult
{
    /// <summary>
    /// Gets or sets field-level validation or binding errors keyed by member name.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IDictionary<string, string[]>? Errors { get; set; }
}
