using System.Reflection;

namespace TodoListApp.Infrastructure.Data;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ApplyDefaultConventions();

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Todo> Todos => Set<Todo>();
}