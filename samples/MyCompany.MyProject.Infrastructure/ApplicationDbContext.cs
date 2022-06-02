using Dddify.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using MyCompany.MyProject.Domain.Entities;

namespace MyCompany.MyProject.Infrastructure
{
    public class ApplicationDbContext : DddifyDbContext
    {
        public ApplicationDbContext(DbContextOptions options, IDbContextDependencies dependencies)
            : base(options, dependencies)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Todo> Todos { get; set; } = default!;

        public DbSet<TodoItem> TodoItems { get; set; } = default!;
    }
}
