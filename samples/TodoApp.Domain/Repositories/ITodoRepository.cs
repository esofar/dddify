using TodoApp.Domain.Aggregates.Todos;

namespace TodoApp.Domain.Repositories;

public interface ITodoRepository : IRepository<Todo, Guid>
{
}