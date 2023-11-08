using Dddify.AspNetCore.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Todolist.Application.Commands;
using Todolist.Application.Dtos;
using Todolist.Application.Queries;
using Todolist.Domain.Entities;

namespace Todolist.WebHost.Controllers
{
    [Route("api/v1/todolists")]
    public class TodoListController : BaseController
    {
        private readonly ISender _sender;

        public TodoListController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// 待办列表
        /// </summary>
        /// <param name="page">当前页</param>
        /// <param name="size">页容量</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult<IPagedResult<TodoListDto>>), StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IPagedResult<TodoListDto>> GetAsync(int page, int size)
        {
            return await SendAsync(new GetTodoListsQuery(page, size));
        }

        /// <summary>
        /// 获取待办
        /// </summary>
        /// <param name="id">待办ID</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult<TodoListDto>), StatusCodes.Status200OK)]
        [HttpGet("{id}")]
        public async Task<TodoListDto> GetAsync([FromRoute] Guid id)
        {
            return await SendAsync(new GetTodoListByIdQuery(id));
        }

        /// <summary>
        /// 创建待办
        /// </summary>
        /// <param name="request">创建字段</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [HttpPost]
        public async Task CreateAsync([FromBody] CreateOrUpdateTodoListDto request)
        {
            await SendAsync(new CreateTodoListCommand(request.Title, request.Colour));
        }

        /// <summary>
        /// 更新待办
        /// </summary>
        /// <param name="id">待办ID</param>
        /// <param name="request">更新字段</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [HttpPut("{id}")]

        public async Task UpdateAsync([FromRoute] Guid id, [FromBody] CreateOrUpdateTodoListDto request)
        {
            await SendAsync(new UpdateTodoListCommand(id, request.Title, request.Colour));

        }

        /// <summary>
        /// 删除待办
        /// </summary>
        /// <param name="id">待办ID</param>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResultWithError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResult), StatusCodes.Status200OK)]
        [HttpDelete("{id}")]
        public async Task DeleteAsync([FromRoute] Guid id)
        {
            await _sender.Send(new DeleteTodoListCommand(id));
        }

        /// <summary>
        /// 支持主题色列表
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(ApiResult<IEnumerable<TodoListColour>>), StatusCodes.Status200OK)]
        [HttpGet("supported-colours")]
        public async Task<IEnumerable<TodoListColour>> GetSupportedColoursAsync()
        {
            return await Task.FromResult(TodoListColour.SupportedColours);
        }

        /// <summary>
        /// 待办事项列表
        /// </summary>
        /// <param name="todoListId">待办ID</param>
        /// <returns></returns>
        [HttpGet("{todoListId}/items")]
        public async Task<IEnumerable<TodoListItemDto>> GetItemsAsync(Guid todoListId)
        {
            return await SendAsync(new GetTodoListItemsQuery(todoListId));
        }

        /// <summary>
        /// 新建待办事项
        /// </summary>
        /// <param name="todoListId">待办ID</param>
        /// <param name="request">新增模型</param>
        /// <returns></returns>
        [HttpPost("{todoListId}/items")]
        public async Task CreateItemAsync(Guid todoListId, [FromBody] CreateOrUpdateTodoListItemDto request)
        {
            await SendAsync(new CreateTodoListItemCommand(
                todoListId,
                request.Note,
                request.Priority));
        }

        /// <summary>
        /// 排序待办事项
        /// </summary>
        /// <param name="todoListId">待办ID</param>
        /// <param name="request">排序字段</param>
        /// <returns></returns>
        [HttpPut("{todoListId}/items")]
        public async Task SortItemsAsync([FromRoute] Guid todoListId, [FromBody] SortTodoListItemDto request)
        {
            await SendAsync(new SortTodoListItemCommand(
                todoListId,
                request.TodoListItemIds));
        }

        /// <summary>
        /// 获取待办事项
        /// </summary>
        /// <param name="todoListId">待办ID</param>
        /// <param name="todoListItemId">待办事项ID</param>
        /// <returns></returns>
        [HttpGet("{todoListId}/items/{todoListItemId}")]
        public async Task<TodoListItemDto> GetItemAsync([FromRoute] Guid todoListId, [FromRoute] Guid todoListItemId)
        {
            return await SendAsync(new GetTodoListItemByIdQuery(
                todoListId,
                todoListItemId));
        }

        /// <summary>
        /// 修改待办事项
        /// </summary>
        /// <param name="todoListId">待办ID</param>
        /// <param name="todoListItemId">待办事项ID</param>
        /// <param name="request">更新字段</param>
        /// <returns></returns>
        [HttpPut("{todoListId}/items/{todoListItemId}")]
        public async Task UpdateItemAsync([FromRoute] Guid todoListId, [FromRoute] Guid todoListItemId, [FromBody] CreateOrUpdateTodoListItemDto request)
        {
            await SendAsync(new UpdateTodoListItemCommand(
                todoListId,
                todoListItemId,
                request.Note,
                request.Priority));
        }

        /// <summary>
        /// 删除待办事项
        /// </summary>
        /// <param name="todoListId">待办ID</param>
        /// <param name="todoListItemId">待办事项ID</param>
        /// <returns></returns>
        [HttpDelete("{todoListId}/items/{todoListItemId}")]
        public async Task DeleteItemAsync(Guid todoListId, Guid todoListItemId)
        {
            await SendAsync(new DeleteTodoListItemCommand(
                todoListId,
                todoListItemId));
        }
    }
}
