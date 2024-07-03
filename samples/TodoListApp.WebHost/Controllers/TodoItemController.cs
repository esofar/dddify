using Dddify.AspNetCore.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Application.Commands;
using TodoListApp.Application.Dtos;
using TodoListApp.Application.Queries;
using TodoListApp.WebHost.Dtos;

namespace TodoListApp.WebHost.Controllers
{
    [ApiController]
    [Route("api/todo-items")]
    public class TodoItemController(ISender sender) : ControllerBase
    {
        /// <summary>
        /// Gets all Todo items.
        /// </summary>
        /// <returns>A collection of Todo item DTOs.</returns>
        [ProducesResponseType(typeof(ApiResult<IPagedResult<TodoItemDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IEnumerable<TodoItemDto>> GetAsync()
        {
            return await sender.Send(new GetAllTodoItemQuery());
        }

        /// <summary>
        /// Creates a Todo item.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
        [HttpPost]
        public async Task PostAsync([FromBody] CreateTodoItemDto request)
        {
            await sender.Send(new CreateTodoItemCommand(request.Text, request.PriorityLevel));
        }

        /// <summary>
        /// Updates a Todo item.
        /// </summary>
        /// <param name="id">The ID of the Todo item to update.</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
        [HttpPut("{id}")]
        public async Task PutAsync([FromRoute] Guid id, [FromBody] UpdateTodoItemDto request)
        {
            await sender.Send(new UpdateTodoItemCommand(id, request.IsDone));
        }

        /// <summary>
        /// Deletes a Todo item by its ID.
        /// </summary>
        /// <param name="id">The ID of the Todo item to delete.</param>
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task DeleteAsync([FromRoute] Guid id)
        {
            await sender.Send(new DeleteTodoItemCommand(id));
        }
    }
}