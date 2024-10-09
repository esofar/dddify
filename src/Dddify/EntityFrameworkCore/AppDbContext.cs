using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Dddify.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly IEnumerable<IInterceptor> _interceptors;

    private static readonly MethodInfo? ConfigurePropertiesMethodInfo =
        typeof(AppDbContext)
            .GetMethod(nameof(ConfigureProperties), BindingFlags.Instance | BindingFlags.NonPublic);

    public AppDbContext(DbContextOptions options, IEnumerable<IInterceptor> interceptors)
        : base(options)
    {
        _interceptors = interceptors;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Model.GetEntityTypes()
            .ForEach(entityType => ConfigurePropertiesMethodInfo?
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]));

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_interceptors);

        base.OnConfiguring(optionsBuilder);
    }

    protected virtual void ConfigureProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType) where TEntity : class
    {
        if (mutableEntityType.IsOwned()) return;

        if (!typeof(TEntity).IsAssignableTo<IEntity>()) return;

        modelBuilder.Entity<TEntity>().ConfigureByConvention();
    }

}


public class AppDbContextOptions : DbContextOptions<AppDbContext>
{
    public string MyProperty { get; set; }

}