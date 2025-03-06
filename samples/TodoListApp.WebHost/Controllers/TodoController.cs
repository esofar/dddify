using Dddify.AspNetCore.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Application.Commands;
using TodoListApp.Application.Dtos;
using TodoListApp.Application.Queries;
using TodoListApp.Domain.Entities;
using TodoListApp.WebHost.Models;

namespace TodoListApp.WebHost.Controllers
{
    [ApiController]
    [Route("api/todos")]
    public class TodoController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Gets all todos.
        /// </summary>
        /// <returns>A collection of todo DTOs.</returns>
        [ProducesResponseType(typeof(ApiResult<IPagedResult<TodoDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IEnumerable<TodoDto>> GetAsync()
        {
            return await sender.Send(new GetAllTodosQuery());
        }

        /// <summary>
        /// Creates a todo.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task<Guid> PostAsync([FromBody] CreateTodoRequest request)
        {
            return await sender.Send(new CreateTodoCommand(request.Text, (TodoPriority)request.Priority));
        }

        /// <summary>
        /// Updates a todo.
        /// </summary>
        /// <param name="id">The ID of the todo to update.</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task PutAsync([FromRoute] Guid id, [FromBody] UpdateTodoRequest request)
        {
            await sender.Send(new UpdateTodoCommand(id, request.IsDone));
        }

        /// <summary>
        /// Deletes a todo.
        /// </summary>
        /// <param name="id">The ID of the todo to delete.</param>
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task DeleteAsync([FromRoute] Guid id)
        {
            await sender.Send(new DeleteTodoCommand(id));
        }
    }
}