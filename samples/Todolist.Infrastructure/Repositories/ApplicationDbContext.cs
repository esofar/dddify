using Dddify.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Todolist.Domain.Entities;
using Todolist.Domain.Repositories;

namespace Todolist.Infrastructure.Repositories;

public class ApplicationDbContext : AppDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions options, InternalInterceptor internalInterceptor)
        : base(options, internalInterceptor)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();
}
