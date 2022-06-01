using Dddify.Infrastructure.EFCore.Extensions;
using Dddify.Infrastructure.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Dddify.Infrastructure.EFCore;

public class DddifyDbContext : DbContext
{
    private readonly IDbContextDependencies _dependencies;

    private static readonly MethodInfo? ConfigureBasePropertiesMethodInfo
     = typeof(DddifyDbContext)
         .GetMethod(
             nameof(ConfigureBaseProperties),
             BindingFlags.Instance | BindingFlags.NonPublic
         );

    public DddifyDbContext(DbContextOptions options, IDbContextDependencies dependencies)
        : base(options)
    {
        _dependencies = dependencies;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetCallingAssembly());

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureBasePropertiesMethodInfo?.MakeGenericMethod(entityType.ClrType)
                .Invoke(this, new object[] { modelBuilder, entityType });
        }

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new DefaultSaveChangesInterceptor(_dependencies));

        base.OnConfiguring(optionsBuilder);
    }

    protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType) where TEntity : class
    {
        if (!mutableEntityType.IsOwned())
        {
            modelBuilder.Entity<TEntity>().ConfigureByConvention();
        }
    }
}
