using Dddify.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Dddify.ResultWrapping.Internal;

/// <summary>
/// Provides the default exception-to-API mapping strategy used by API result wrapping.
/// </summary>
public class DefaultApiExceptionMapper(
    IOptions<ApiResultWrappingOptions> options,
    IBusinessExceptionResourceTypeResolver businessExceptionResourceTypeResolver,
    IStringLocalizerFactory? localizerFactory = null) : IApiExceptionMapper
{
    private readonly ApiResultWrappingOptions _options = options.Value;

    /// <summary>
    /// Maps an exception into an API-facing error payload description.
    /// </summary>
    /// <param name="exception">The exception to map.</param>
    public ApiExceptionMapping Map(Exception exception)
        => exception switch
        {
            BusinessException businessException => MapBusinessException(businessException, _options.BusinessException.StatusCode),
            ValidationException validationException => new ApiExceptionMapping
            {
                StatusCode = _options.ValidationException.StatusCode,
                ErrorCode = _options.ValidationException.ErrorCode,
                ErrorMessage = ResolveSharedErrorMessage(_options.ValidationException.ErrorCode),
                Errors = ResolveValidationErrors(validationException.Errors),
            },
            DbUpdateConcurrencyException => new ApiExceptionMapping
            {
                StatusCode = _options.ConcurrencyException.StatusCode,
                ErrorCode = _options.ConcurrencyException.ErrorCode,
                ErrorMessage = ResolveSharedErrorMessage(_options.ConcurrencyException.ErrorCode),
            },
            _ => new ApiExceptionMapping
            {
                StatusCode = _options.InternalServerException.StatusCode,
                ErrorCode = _options.InternalServerException.ErrorCode,
                ErrorMessage = ResolveSharedErrorMessage(_options.InternalServerException.ErrorCode),
            }
        };

    private ApiExceptionMapping MapBusinessException(BusinessException exception, int statusCode)
        => new()
        {
            StatusCode = statusCode,
            ErrorCode = exception.ErrorCode,
            ErrorMessage = ResolveErrorMessage(exception),
            Metadata = ResolveMetadata(exception, statusCode),
        };

    /// <summary>
    /// Resolves the user-facing message for a business exception by using a resolved resource type and error code.
    /// </summary>
    private string? ResolveErrorMessage(BusinessException exception)
    {
        var errorCode = exception.ErrorCode;
        if (string.IsNullOrWhiteSpace(errorCode))
        {
            return null;
        }

        if (localizerFactory is null)
        {
            return errorCode;
        }

        var resourceType = businessExceptionResourceTypeResolver.Resolve(exception);
        var localizer = localizerFactory.Create(resourceType);
        var localized = localizer[errorCode, exception.ErrorArgs];
        return localized.ResourceNotFound ? errorCode : localized.Value;
    }

    /// <summary>
    /// Resolves a shared fallback message from the shared resource set.
    /// </summary>
    private string ResolveSharedErrorMessage(string errorCode)
    {
        if (localizerFactory is null)
        {
            return errorCode;
        }

        var localizer = localizerFactory.Create(typeof(Localization.SharedResource));
        var localized = localizer[errorCode];
        return localized.ResourceNotFound ? errorCode : localized.Value;
    }

    /// <summary>
    /// Converts FluentValidation failures into field-level error payloads.
    /// </summary>
    private static Dictionary<string, string[]> ResolveValidationErrors(IEnumerable<ValidationFailure> failures)
        => failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(group => group.Key, group => group.ToArray());

    /// <summary>
    /// Builds structured diagnostic metadata for a business exception.
    /// </summary>
    private static Dictionary<string, object?> ResolveMetadata(BusinessException exception, int statusCode)
    {
        var metadata = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            ["Category"] = exception.Category,
            ["ErrorCode"] = exception.ErrorCode,
            ["StatusCode"] = statusCode,
        };

        foreach (var entry in exception.Metadata)
        {
            metadata[entry.Key] = entry.Value;
        }

        return metadata;
    }
}
