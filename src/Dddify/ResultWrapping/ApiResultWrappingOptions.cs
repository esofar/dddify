using Dddify.ResultWrapping.Internal;

namespace Dddify.ResultWrapping;

/// <summary>
/// Configures how API result wrapping and framework-level error projection behave.
/// </summary>
public class ApiResultWrappingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether the response payload should include the current trace identifier.
    /// </summary>
    /// <remarks>
    /// Default value is <c>true</c>.
    /// </remarks>
    public bool EnableTraceIdentifier { get; set; } = true;

    /// <summary>
    /// Gets the options used when mapping validation exceptions.
    /// </summary>
    public ApiExceptionResultOptions ValidationException { get; } = new("validation_failed", 400);

    /// <summary>
    /// Gets the options used when mapping concurrency exceptions.
    /// </summary>
    public ApiExceptionResultOptions ConcurrencyException { get; } = new("concurrency_conflict", 409);

    /// <summary>
    /// Gets the options used when mapping unexpected server-side exceptions.
    /// </summary>
    public ApiExceptionResultOptions InternalServerException { get; } = new("internal_server_error", 500);

    /// <summary>
    /// Gets the options used when mapping business exceptions, including domain and application exceptions.
    /// </summary>
    public BusinessExceptionResultOptions BusinessException { get; } = new();
}

/// <summary>
/// Configures the API-facing projection for an exception type with a fixed error code.
/// </summary>
/// <param name="errorCode">The default error code.</param>
/// <param name="statusCode">The default HTTP status code.</param>
public sealed class ApiExceptionResultOptions(string errorCode, int statusCode)
{
    /// <summary>
    /// Gets or sets the error code used for the exception result.
    /// </summary>
    public string ErrorCode { get; set; } = errorCode;

    /// <summary>
    /// Gets or sets the HTTP status code used for the exception result.
    /// </summary>
    public int StatusCode { get; set; } = statusCode;
}

/// <summary>
/// Configures the API-facing projection for business exceptions.
/// </summary>
public sealed class BusinessExceptionResultOptions
{
    /// <summary>
    /// Gets or sets the HTTP status code used for business exceptions.
    /// </summary>
    /// <remarks>
    /// Default value is <c>200</c>.
    /// </remarks>
    public int StatusCode { get; set; } = 200;

    /// <summary>
    /// Gets the resolver type used to determine the localization resource type for business exceptions.
    /// </summary>
    internal Type ResourceTypeResolverType { get; private set; } =
        typeof(DefaultBusinessExceptionResourceTypeResolver);

    /// <summary>
    /// Sets the resolver type used to determine the localization resource type for business exceptions.
    /// </summary>
    /// <typeparam name="TResolver">The resolver implementation type.</typeparam>
    /// <returns>The current <see cref="BusinessExceptionResultOptions"/> instance.</returns>
    public BusinessExceptionResultOptions UseResourceTypeResolver<TResolver>()
        where TResolver : class, IBusinessExceptionResourceTypeResolver
    {
        ResourceTypeResolverType = typeof(TResolver);
        return this;
    }
}
