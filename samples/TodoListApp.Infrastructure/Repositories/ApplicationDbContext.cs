using Dddify.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TodoListApp.Domain.Entities;
using TodoListApp.Domain.Repositories;

namespace TodoListApp.Infrastructure.Repositories;

public class ApplicationDbContext(DbContextOptions options, IEnumerable<IInterceptor> interceptors)
    : AppDbContext(options, interceptors), IApplicationDbContext
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();
}
