using Dddify.Domain;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Domain.Repositories;

public interface ITodoRepository : IRepository<Todo, Guid>
{
    Task<IEnumerable<Todo>> GetSortedTodosAsync();
}