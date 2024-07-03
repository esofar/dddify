using Dddify.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Domain.Entities;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Infrastructure.Repositories;

public class ApplicationDbContext(
    DbContextOptions options,
    InternalInterceptor internalInterceptor) : AppDbContext(options, internalInterceptor), IApplicationDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
