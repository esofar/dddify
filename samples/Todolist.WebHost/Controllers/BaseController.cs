using Dddify.Messaging.Commands;
using Dddify.Messaging.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Todolist.WebHost.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected ISender Sender => HttpContext.RequestServices.GetRequiredService<ISender>();

    protected async Task<TResult> SendAsync<TResult>(IQuery<TResult> query)
    {
        return await Sender.Send(query, HttpContext.RequestAborted);
    }

    protected async Task<TResult> SendAsync<TResult>(ICommand<TResult> command)
    {
        return await Sender.Send(command, HttpContext.RequestAborted);
    }

    protected async Task SendAsync(ICommand command)
    {
        await Sender.Send(command, HttpContext.RequestAborted);
    }
}