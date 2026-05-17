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
        LogException(context.Exception, mapping);

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

    private void LogException(Exception exception, ApiExceptionMapping mapping)
    {
        using var scope = logger.BeginScope(BuildScope(mapping, exception));

        if (exception is BusinessException bizException && logger.IsEnabled(bizException.LogLevel))
        {
            logger.Log(bizException.LogLevel, "{Category} exception occurred.", bizException.Category);
            return;
        }

        logger.LogError(exception, "Unhandled exception occurred.");
    }

    private string? ResolveTraceId(string traceId)
        => _options.EnableTraceIdentifier ? traceId : null;

    /// <summary>
    /// Builds the logging scope used when writing exception logs for wrapped API responses.
    /// </summary>
    private static ApiExceptionLogScope BuildScope(ApiExceptionMapping mapping, Exception exception)
    {
        var values = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["ExceptionType"] = exception.GetType().Name,
            ["ErrorCode"] = mapping.ErrorCode,
            ["StatusCode"] = mapping.StatusCode,
        };

        if (mapping.Metadata is not null)
        {
            foreach (var pair in mapping.Metadata)
            {
                values[pair.Key] = pair.Value;
            }
        }

        return new ApiExceptionLogScope(values);
    }

    private sealed class ApiExceptionLogScope(IReadOnlyDictionary<string, object?> values) : IReadOnlyList<KeyValuePair<string, object?>>
    {
        private readonly IReadOnlyList<KeyValuePair<string, object?>> _values = [.. values];

        public int Count => _values.Count;

        public KeyValuePair<string, object?> this[int index] => _values[index];

        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
            => _values.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => GetEnumerator();

        public override string ToString()
            => string.Join(", ", _values.Select(pair => $"{pair.Key}={FormatValue(pair.Value)}"));

        private static string FormatValue(object? value)
        {
            if (value is null)
            {
                return "<null>";
            }

            if (value is string text)
            {
                return text;
            }

            if (value is System.Collections.IEnumerable values)
            {
                return $"[{string.Join(", ", values.Cast<object?>().Select(FormatValue))}]";
            }

            return value.ToString() ?? string.Empty;
        }
    }
}
