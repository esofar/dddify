using Dddify.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Converts unhandled exceptions from API actions into wrapped API result payloads.
/// </summary>
public class ApiExceptionFilter(
    ILogger<ApiExceptionFilter> logger,
    IApiExceptionMapper exceptionMapper,
    IOptions<ApiResultWrappingOptions> options) : IAsyncExceptionFilter
{
    private readonly ApiResultWrappingOptions _options = options.Value;

    /// <summary>
    /// Handles an unhandled exception and replaces the action result with a wrapped error payload.
    /// </summary>
    /// <param name="context">The exception context.</param>
    public Task OnExceptionAsync(ExceptionContext context)
    {
        var mapping = exceptionMapper.Map(context.Exception);
        var traceId = ResolveTraceId(context.HttpContext.TraceIdentifier);
        LogException(context.Exception, mapping, traceId);

        var payload = mapping.Errors is { Count: > 0 }
            ? ApiResultFactory.Failure(mapping.Errors, mapping.ErrorCode, mapping.ErrorMessage, traceId)
            : ApiResultFactory.Failure(mapping.ErrorCode, mapping.ErrorMessage, traceId);

        context.Result = new ObjectResult(payload)
        {
            StatusCode = mapping.StatusCode
        };
        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }

    private void LogException(Exception exception, ApiExceptionMapping mapping, string? traceId)
    {
        var scopeState = BuildScope(mapping, exception, traceId);
        using var scope = logger.BeginScope(scopeState);

        if (exception is BusinessException bizException)
        {
            logger.Log(bizException.LogLevel, bizException, "{Category} exception occurred.", bizException.Category);
            return;
        }

        logger.LogError(exception, "Unhandled exception occurred.");
    }

    private string? ResolveTraceId(string traceId)
        => _options.EnableTraceIdentifier ? traceId : null;

    /// <summary>
    /// Builds the logging scope used when writing exception logs for wrapped API responses.
    /// </summary>
    private static Dictionary<string, object?> BuildScope(ApiExceptionMapping mapping, Exception exception, string? traceId)
    {
        var scope = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["ExceptionType"] = exception.GetType().Name,
            ["ErrorCode"] = mapping.ErrorCode,
            ["StatusCode"] = mapping.StatusCode,
        };

        if (!string.IsNullOrWhiteSpace(traceId))
        {
            scope["TraceId"] = traceId;
        }

        if (mapping.Metadata is null)
        {
            return scope;
        }

        foreach (var pair in mapping.Metadata)
        {
            scope[pair.Key] = pair.Value;
        }

        return scope;
    }
}
