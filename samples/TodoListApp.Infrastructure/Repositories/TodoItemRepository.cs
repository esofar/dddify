using Dddify.EntityFrameworkCore;
using TodoListApp.Domain.Entities;
using TodoListApp.Domain.Repositories;
using TodoListApp.Infrastructure.Contexts;

namespace TodoListApp.Infrastructure.Repositories;

public class TodoItemRepository(ApplicationDbContext context)
    : Repository<ApplicationDbContext, TodoItem, Guid>(context), ITodoItemRepository
{
}