using Dddify.Domain;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Domain.Repositories;

public interface ITodoItemRepository : IRepository<TodoItem, Guid>
{
}