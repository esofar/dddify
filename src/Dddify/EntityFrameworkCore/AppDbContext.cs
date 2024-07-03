using Dddify.Domain;
using Dddify.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Dddify.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    private readonly InternalInterceptor _internalInterceptor;

    private static readonly MethodInfo? ConfigureBasePropertiesMethodInfo
     = typeof(AppDbContext)
         .GetMethod(
             nameof(ConfigureBaseProperties),
             BindingFlags.Instance | BindingFlags.NonPublic
         );

    public AppDbContext(DbContextOptions options, InternalInterceptor internalInterceptor)
        : base(options)
    {
        _internalInterceptor = internalInterceptor;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            ConfigureBasePropertiesMethodInfo?.MakeGenericMethod(entityType.ClrType)
                .Invoke(this, [modelBuilder, entityType]);
        }

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

    /// <summary>
    /// Resets the concurrency stamp of an entity to the specified value.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity whose concurrency stamp needs to be reset.</param>
    /// <param name="concurrencyStamp">The new concurrency stamp value.</param>
    /// <remarks>
    /// This method is used to update the concurrency stamp of an entity to a new value.
    /// The concurrency stamp is typically used to track changes and detect conflicts
    /// when multiple users are modifying the same entity concurrently.
    /// </remarks>
    public void ResetConcurrencyStamp<TEntity>(TEntity entity, string concurrencyStamp)
        where TEntity : IEntity
    {
        Entry(entity).Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp)).OriginalValue = concurrencyStamp;
    }
}