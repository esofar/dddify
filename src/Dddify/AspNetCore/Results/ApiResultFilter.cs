using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dddify.AspNetCore.Results;

public class ApiResultFilter(IApiResultWrapper apiResultWrapper) : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var isApiController = context.ActionDescriptor.HasAttrbute<ApiControllerAttribute>();
        var disableApiResultWrapper = context.ActionDescriptor.HasAttrbute<DontWrapResultAttribute>();

        if (isApiController && !disableApiResultWrapper)
        {
            context.Result = context.Result switch
            {
                ObjectResult objectResult => new(apiResultWrapper.Succeed(objectResult.Value)),
                EmptyResult _ => new(apiResultWrapper.Succeed()),
                _ => new ObjectResult(apiResultWrapper.Failed()),
            };
        }

        await next();
    }
}