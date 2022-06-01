using Dddify.AspNetCore.ApiResult;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyCompany.MyProject.Application.Todos.Commands;
using MyCompany.MyProject.Application.Todos.Queries;
using MyCompany.MyProject.Domain.ValueObjects;

namespace MyCompany.MyProject.Web.Controllers;

[ApiController]
[Route("api/v1/todos")]
public class TodoController : ControllerBase
{
    private readonly ISender _sender;

    public TodoController(ISender sender)
    {
        _sender = sender;
    }

    [ProducesResponseType(typeof(ApiResult<IPagedList<TodoDto>>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<IPagedList<TodoDto>> GetPagedListAsync(int page, int size)
    {
        return await _sender.Send(new GetPagedTodosQuery { Page = page, Size = size }, HttpContext.RequestAborted);
    }

    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status200OK)]
    [HttpGet("{id}")]
    public async Task<TodoDto> GetByIdAsync(Guid id)
    {
        return await _sender.Send(new GetTodoQuery { Id = id }, HttpContext.RequestAborted);
    }

    [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status200OK)]
    [HttpPost]
    public async Task<TodoDto> CreateAsync(CreateTodoCommand command)
    {
        return await _sender.Send(command, HttpContext.RequestAborted);
    }

    [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult<TodoDto>), StatusCodes.Status200OK)]
    [HttpPut]
    public async Task<TodoDto> UpdateAsync(UpdateTodoCommand command)
    {
        return await _sender.Send(command, HttpContext.RequestAborted);
    }

    [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
    [HttpDelete("{id}")]
    public async Task DeleteAsync(Guid id)
    {
        await _sender.Send(new DeleteTodoCommand { Id = id }, HttpContext.RequestAborted);
    }

    [ProducesResponseType(typeof(ApiResult<IEnumerable<Colour>>), StatusCodes.Status200OK)]
    [HttpGet("supported-colours")]
    public async Task<IEnumerable<Colour>> GetSupportedColoursAsync()
    {
        return await Task.FromResult(Colour.SupportedColours);
    }
}
