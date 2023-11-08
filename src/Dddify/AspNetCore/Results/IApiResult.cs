namespace Dddify.AspNetCore.Results;

public interface IApiResult
{
    /// <summary>
    /// Indicates the operation is success.
    /// </summary>
    bool Success { get; set; }

    /// <summary>
    /// The code of business operation error.
    /// </summary>
    string? ErrorCode { get; set; }

    /// <summary>
    /// The message display to user.
    /// </summary>
    string? ErrorMessage { get; set; }

    /// <summary>
    /// The unique request ID.
    /// </summary>
    string? TraceId { get; set; }
}

public interface IApiResult<T> : IApiResult
{
    /// <summary>
    /// The result data of this request.
    /// </summary>
    T? Data { get; set; }
}

public interface IApiResultWithError : IApiResult
{
    /// <summary>
    /// The error of validation failures.
    /// </summary>
    IDictionary<string, string[]>? Error { get; set; }
}