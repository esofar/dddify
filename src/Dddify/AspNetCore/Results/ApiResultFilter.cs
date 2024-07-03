﻿using Dddify.System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dddify.AspNetCore.Results;

public class ApiResultFilter : IAsyncResultFilter
{
    private readonly IApiResultWrapper _apiResultWrapper;

    public ApiResultFilter(IApiResultWrapper apiResultWrapper)
    {
        _apiResultWrapper = apiResultWrapper;
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        var isApiController = context.ActionDescriptor.HasAttrbute<ApiControllerAttribute>();
        var disableApiResultWrapper = context.ActionDescriptor.HasAttrbute<DontWrapResultAttribute>();

        if (isApiController && !disableApiResultWrapper)
        {
            context.Result = context.Result switch
            {
                ObjectResult objectResult => new(_apiResultWrapper.Succeed(objectResult.Value)),
                EmptyResult _ => new(_apiResultWrapper.Succeed()),
                _ => new ObjectResult(_apiResultWrapper.Failed()),
            };
        }

        await next();
    }
}