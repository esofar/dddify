using Dddify.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TodoListApp.Domain.Entities;

namespace TodoListApp.Infrastructure.Contexts;

public class ApplicationDbContext(DbContextOptions options) : AppDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}