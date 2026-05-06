using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Wraps known JSON/API Web API action results into the standard API result envelope.
/// </summary>
public class ApiResultFilter(IOptions<ApiResultWrappingOptions> options) : IAsyncResultFilter
{
    private const string DefaultBadRequestMessage = "Bad Request";
    private const string DefaultRequestFailedMessage = "Request failed";

    private readonly ApiResultWrappingOptions _options = options.Value;

    /// <summary>
    /// Rewrites the current action result into a wrapped API result payload when applicable.
    /// </summary>
    /// <param name="context">The result execution context.</param>
    /// <param name="next">The delegate that proceeds with the remaining pipeline.</param>
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (ShouldWrap(context.ActionDescriptor))
        {
            var traceId = ResolveTraceId(context.HttpContext.TraceIdentifier);
            context.Result = WrapResult(context.Result, traceId);
        }

        await next();
    }

    /// <summary>
    /// Determines whether result wrapping should run for the specified action descriptor.
    /// </summary>
    private static bool ShouldWrap(ActionDescriptor actionDescriptor)
        => HasAttribute<ApiControllerAttribute>(actionDescriptor)
            && !HasAttribute<DisableResultWrappingAttribute>(actionDescriptor);

    /// <summary>
    /// Determines whether the specified endpoint metadata contains the given attribute type.
    /// </summary>
    private static bool HasAttribute<T>(ActionDescriptor target)
        where T : Attribute
        => target.EndpointMetadata.Any(metadata => metadata is T);

    /// <summary>
    /// Wraps a known JSON/API action result into the standard API result envelope.
    /// </summary>
    private IActionResult WrapResult(IActionResult result, string? traceId)
        => result switch
        {
            BadRequestObjectResult badRequestObject => WrapBadRequestObjectResult(badRequestObject, traceId),
            BadRequestResult => CreateFailureResult(StatusCodes.Status400BadRequest, traceId, errorMessage: DefaultBadRequestMessage),
            ObjectResult objectResult => WrapObjectResult(objectResult, traceId),
            NoContentResult => CreateSuccessResult(StatusCodes.Status204NoContent, traceId),
            StatusCodeResult statusCodeResult when statusCodeResult.StatusCode is >= 400 and < 600
                => CreateFailureResult(
                    statusCodeResult.StatusCode,
                    traceId,
                    ResolveDefaultErrorCode(statusCodeResult.StatusCode),
                    ResolveDefaultErrorMessage(statusCodeResult.StatusCode)),
            StatusCodeResult statusCodeResult => CreateSuccessResult(statusCodeResult.StatusCode, traceId),
            EmptyResult => CreateSuccessResult(StatusCodes.Status200OK, traceId),
            _ => result
        };

    /// <summary>
    /// Wraps a <see cref="BadRequestObjectResult"/> into the standard API result envelope.
    /// </summary>
    private ObjectResult WrapBadRequestObjectResult(BadRequestObjectResult result, string? traceId)
        => TryWrapErrorPayload(result.Value, StatusCodes.Status400BadRequest, traceId, DefaultBadRequestMessage)
            ?? CreateFailureResult(StatusCodes.Status400BadRequest, traceId, errorMessage: DefaultBadRequestMessage);

    /// <summary>
    /// Wraps an <see cref="ObjectResult"/> into the standard API result envelope.
    /// </summary>
    private ObjectResult WrapObjectResult(ObjectResult objectResult, string? traceId)
    {
        var statusCode = objectResult.StatusCode ?? TryGetProblemStatus(objectResult.Value) ?? InferSuccessStatus(objectResult);

        var wrappedError = TryWrapErrorPayload(objectResult.Value, statusCode, traceId, DefaultRequestFailedMessage);
        if (wrappedError is not null)
        {
            return wrappedError;
        }

        if (statusCode is >= 400 and < 600)
        {
            return CreateFailureResult(
                statusCode,
                traceId,
                ResolveDefaultErrorCode(statusCode),
                ResolveDefaultErrorMessage(statusCode));
        }

        return new ObjectResult(ApiResultFactory.Success(objectResult.Value, traceId))
        {
            StatusCode = statusCode
        };
    }

    /// <summary>
    /// Attempts to convert a known MVC error payload into the standard API result envelope.
    /// </summary>
    private ObjectResult? TryWrapErrorPayload(object? value, int statusCode, string? traceId, string defaultMessage)
        => value switch
        {
            ValidationProblemDetails validationProblem => CreateValidationFailureResult(
                validationProblem.Errors,
                validationProblem.Detail ?? validationProblem.Title,
                traceId),
            SerializableError serializableError => CreateValidationFailureResult(
                ToValidationErrors(serializableError),
                _options.ValidationException.ErrorCode,
                traceId),
            ModelStateDictionary modelState => CreateValidationFailureResult(
                ToValidationErrors(modelState),
                _options.ValidationException.ErrorCode,
                traceId),
            ProblemDetails problemDetails => CreateFailureResult(
                statusCode,
                traceId,
                GetProblemCode(problemDetails) ?? ResolveDefaultErrorCode(statusCode),
                problemDetails.Detail ?? problemDetails.Title ?? defaultMessage),
            _ => null
        };

    /// <summary>
    /// Creates a standardized validation failure payload.
    /// </summary>
    private ObjectResult CreateValidationFailureResult(
        IDictionary<string, string[]> errors,
        string? errorMessage,
        string? traceId)
        => new(ApiResultFactory.Failure(errors, _options.ValidationException.ErrorCode, errorMessage ?? _options.ValidationException.ErrorCode, traceId))
        {
            StatusCode = _options.ValidationException.StatusCode
        };

    /// <summary>
    /// Converts <see cref="SerializableError"/> into field-level validation errors.
    /// </summary>
    private static Dictionary<string, string[]> ToValidationErrors(SerializableError serializableError)
        => serializableError.ToDictionary(
            entry => entry.Key,
            entry => entry.Value switch
            {
                string[] items => items,
                string message => [message],
                _ => [entry.Value?.ToString() ?? DefaultBadRequestMessage]
            });

    /// <summary>
    /// Converts <see cref="ModelStateDictionary"/> into field-level validation errors.
    /// </summary>
    private static Dictionary<string, string[]> ToValidationErrors(ModelStateDictionary modelState)
        => modelState
            .Where(item => item.Value is { Errors.Count: > 0 })
            .ToDictionary(
                item => item.Key,
                item => item.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

    /// <summary>
    /// Infers the default success status code for an <see cref="ObjectResult"/>.
    /// </summary>
    private static int InferSuccessStatus(ObjectResult objectResult)
        => objectResult switch
        {
            OkObjectResult => StatusCodes.Status200OK,
            CreatedResult or CreatedAtActionResult or CreatedAtRouteResult => StatusCodes.Status201Created,
            AcceptedResult or AcceptedAtActionResult or AcceptedAtRouteResult => StatusCodes.Status202Accepted,
            _ => StatusCodes.Status200OK
        };

    /// <summary>
    /// Attempts to read the status code from a <see cref="ProblemDetails"/> payload.
    /// </summary>
    private static int? TryGetProblemStatus(object? value)
        => value is ProblemDetails problemDetails ? problemDetails.Status : null;

    /// <summary>
    /// Resolves the default error code for framework-generated error results.
    /// </summary>
    private string? ResolveDefaultErrorCode(int statusCode)
        => statusCode switch
        {
            StatusCodes.Status409Conflict => _options.ConcurrencyException.ErrorCode,
            StatusCodes.Status500InternalServerError => _options.InternalServerException.ErrorCode,
            _ => null
        };

    /// <summary>
    /// Resolves the default error message for framework-generated error results.
    /// </summary>
    private string? ResolveDefaultErrorMessage(int statusCode)
        => ResolveDefaultErrorCode(statusCode);

    /// <summary>
    /// Reads the custom problem code from <see cref="ProblemDetails.Extensions"/> when present.
    /// </summary>
    private static string? GetProblemCode(ProblemDetails problemDetails)
        => problemDetails.Extensions.TryGetValue("code", out var codeValue)
            && codeValue is string code
            && !string.IsNullOrWhiteSpace(code)
                ? code
                : null;

    /// <summary>
    /// Creates a standardized wrapped success result.
    /// </summary>
    private static ObjectResult CreateSuccessResult(int statusCode, string? traceId)
        => new(ApiResultFactory.Success(traceId))
        {
            StatusCode = statusCode
        };

    /// <summary>
    /// Creates a standardized wrapped failure result.
    /// </summary>
    private static ObjectResult CreateFailureResult(int statusCode, string? traceId, string? errorCode = null, string? errorMessage = null)
        => new(ApiResultFactory.Failure(errorCode, errorMessage, traceId))
        {
            StatusCode = statusCode
        };

    /// <summary>
    /// Resolves the trace identifier according to the current wrapping options.
    /// </summary>
    private string? ResolveTraceId(string traceId)
        => _options.EnableTraceIdentifier ? traceId : null;
}
