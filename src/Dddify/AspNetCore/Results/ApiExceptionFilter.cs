using Dddify.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Dddify.AspNetCore.Results;

public class ApiExceptionFilter : IAsyncExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    private readonly IApiResultWrapper _apiResultWrapper;
    private readonly IStringLocalizerFactory _localizerFactory;
    private readonly Dictionary<Type, Func<ExceptionContext, ObjectResult>> _exceptionHandlers;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IApiResultWrapper apiResultWrapper, IStringLocalizerFactory localizerFactory)
    {
        _logger = logger;
        _apiResultWrapper = apiResultWrapper;
        _localizerFactory = localizerFactory;

        _exceptionHandlers = new Dictionary<Type, Func<ExceptionContext, ObjectResult>>
        {
            { typeof(BadRequestException), HandleBadRequestException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedException), HandleUnauthorizedException },
            { typeof(ForbiddenException), HandleForbiddenException },
        };
    }

    public Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is KnownException exception)
        {
            _logger.Log(exception.LogLevel, exception, "A known (application or domain) exception occurred.");

            context.Result = HandleKnownException(exception);
        }
        else
        {
            _logger.LogError(context.Exception, "A unknown exception occurred.");

            context.Result = _exceptionHandlers.TryGetValue(context.Exception.GetType(), out Func<ExceptionContext, ObjectResult>? handler)
                ? handler.Invoke(context)
                : HandleUnknownException();
        }

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }

    private ObjectResult HandleKnownException(KnownException exception)
    {
        var localizer = _localizerFactory.Create(exception.ResourceType);

        var apiResult = string.IsNullOrEmpty(exception.LocalizedName)
            ? _apiResultWrapper.Failed()
            : _apiResultWrapper.Failed(exception.Arguments.Length != 0
                ? localizer[exception.LocalizedName, exception.Arguments]
                : localizer[exception.LocalizedName]);

        return new ObjectResult(apiResult);
    }

    private ObjectResult HandleBadRequestException(ExceptionContext context)
    {
        var exception = context.Exception as BadRequestException;

        var apiResult = _apiResultWrapper.Failed(exception!.Errors);

        return new BadRequestObjectResult(apiResult);
    }

    private NotFoundObjectResult HandleNotFoundException(ExceptionContext context)
    {
        var apiResult = _apiResultWrapper.Failed(context.Exception.Message);

        return new NotFoundObjectResult(apiResult);
    }

    private ObjectResult HandleUnauthorizedException(ExceptionContext context)
    {
        var apiResult = _apiResultWrapper.Failed(context.Exception.Message);

        return new ObjectResult(apiResult)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }

    private ObjectResult HandleForbiddenException(ExceptionContext context)
    {
        var apiResult = _apiResultWrapper.Failed(context.Exception.Message);

        return new ObjectResult(apiResult)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    private ObjectResult HandleUnknownException()
    {
        var apiResult = _apiResultWrapper.Failed();

        return new ObjectResult(apiResult)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}