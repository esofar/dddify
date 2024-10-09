using Dddify.EntityFrameworkCore;
using TodoListApp.Domain.Entities;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Infrastructure.Repositories;

public class TodoItemRepository : Repository<ApplicationDbContext, TodoItem>, ITodoItemRepository
{
    private readonly ApplicationDbContext _context;

    public TodoItemRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}