using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Dddify.EntityFrameworkCore;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    private static readonly MethodInfo? ConfigurePropertiesMethodInfo =
        typeof(AppDbContext)
            .GetMethod(nameof(ConfigureProperties), BindingFlags.Instance | BindingFlags.NonPublic);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Model.GetEntityTypes()
            .ForEach(entityType => ConfigurePropertiesMethodInfo?
                .MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]));

        base.OnModelCreating(modelBuilder);
    }

    protected virtual void ConfigureProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType)
        where TEntity : class, IEntity
    {
        if (!mutableEntityType.IsOwned())
        {
            modelBuilder.Entity<TEntity>().ConfigureByConvention();
        }
    }
}