using Dddify.Exceptions;
using Dddify.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Dddify.AspNetCore.ApiResult;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ApiExceptionFilter> _logger;
    private readonly IApiResultWrapper _apiResultWrapper;
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger, IApiResultWrapper apiResultWrapper)
    {
        _logger = logger;
        _apiResultWrapper = apiResultWrapper;

        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            { typeof(BussinessException), HandleBussinessException },
            { typeof(BadRequestException), HandleBadRequestException },
            { typeof(NotFoundException), HandleNotFoundException },
            { typeof(UnauthorizedException), HandleUnauthorizedException },
            { typeof(ForbiddenException), HandleForbiddenException },
        };
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

        var type = context.Exception.GetType();

        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
        }
        else
        {
            HandleUnknownException(context);
        }

        context.ExceptionHandled = true;
    }

    private void HandleBussinessException(ExceptionContext context)
    {
        var exception = context.Exception as BussinessException;
        var apiResult = _apiResultWrapper.Failed(exception!.Message);

        context.Result = new ObjectResult(apiResult);
    }

    private void HandleBadRequestException(ExceptionContext context)
    {
        var exception = context.Exception as BadRequestException;
        var apiResult = _apiResultWrapper.Failed(exception!.Errors);

        context.Result = new BadRequestObjectResult(apiResult);
    }

    private void HandleNotFoundException(ExceptionContext context)
    {
        var exception = context.Exception as NotFoundException;
        var apiResult = _apiResultWrapper.Failed(exception!.Message);

        context.Result = new NotFoundObjectResult(apiResult);
    }

    private void HandleUnauthorizedException(ExceptionContext context)
    {
        var exception = context.Exception as UnauthorizedException;
        var apiResult = _apiResultWrapper.Failed(exception!.Message);

        context.Result = new ObjectResult(apiResult)
        {
            StatusCode = StatusCodes.Status401Unauthorized
        };
    }

    private void HandleForbiddenException(ExceptionContext context)
    {
        var exception = context.Exception as ForbiddenException;
        var apiResult = _apiResultWrapper.Failed(exception!.Message);

        context.Result = new ObjectResult(apiResult)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };
    }

    private void HandleUnknownException(ExceptionContext context)
    {
        var apiResult = _apiResultWrapper.Failed("An error occurred while processing your request.");

        context.Result = new ObjectResult(apiResult)
        {
            StatusCode = StatusCodes.Status500InternalServerError
        };
    }
}
