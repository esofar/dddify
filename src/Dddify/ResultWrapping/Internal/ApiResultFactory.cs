namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Creates standardized API result payload instances for successful and failed responses.
/// </summary>
public static class ApiResultFactory
{
    /// <summary>
    /// Creates a successful result payload without data.
    /// </summary>
    /// <param name="traceId">The trace identifier to include in the payload.</param>
    public static ApiResult Success(string? traceId = null)
        => new()
        {
            Success = true,
            TraceId = traceId,
        };

    /// <summary>
    /// Creates a successful result payload with data.
    /// </summary>
    /// <typeparam name="T">The payload type.</typeparam>
    /// <param name="data">The response data.</param>
    /// <param name="traceId">The trace identifier to include in the payload.</param>
    public static ApiResult<T> Success<T>(T? data, string? traceId = null)
        => new()
        {
            Success = true,
            TraceId = traceId,
            Data = data,
        };

    /// <summary>
    /// Creates a failed result payload without field-level errors.
    /// </summary>
    /// <param name="errorCode">The stable error code.</param>
    /// <param name="errorMessage">The user-facing error message.</param>
    /// <param name="traceId">The trace identifier to include in the payload.</param>
    public static ApiResult Failure(string? errorCode = null, string? errorMessage = null, string? traceId = null)
        => new()
        {
            Success = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            TraceId = traceId,
        };

    /// <summary>
    /// Creates a failed result payload with field-level errors.
    /// </summary>
    /// <param name="errors">The field-level errors keyed by member name.</param>
    /// <param name="errorCode">The stable error code.</param>
    /// <param name="errorMessage">The user-facing error message.</param>
    /// <param name="traceId">The trace identifier to include in the payload.</param>
    public static ApiResultWithErrors Failure(
        IDictionary<string, string[]> errors,
        string? errorCode = null,
        string? errorMessage = null,
        string? traceId = null)
        => new()
        {
            Success = false,
            ErrorCode = errorCode,
            ErrorMessage = errorMessage,
            TraceId = traceId,
            Errors = errors,
        };
}
