using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Dddify.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly InternalInterceptor _internalInterceptor;

    private static readonly MethodInfo? ConfigureBasePropertiesMethodInfo
        = typeof(AppDbContext)
            .GetMethod(nameof(ConfigureBaseProperties), BindingFlags.Instance | BindingFlags.NonPublic);

    public AppDbContext(DbContextOptions options, InternalInterceptor internalInterceptor)
        : base(options)
    {
        _internalInterceptor = internalInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Model.GetEntityTypes()
            .ForEach(entityType => ConfigureBasePropertiesMethodInfo?
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]));

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_internalInterceptor);

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