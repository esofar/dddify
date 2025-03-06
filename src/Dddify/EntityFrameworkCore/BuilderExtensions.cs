using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for configuring entity types.
/// </summary>
public static class BuilderExtensions
{
    /// <summary>
    /// Configures the entity type to support soft deletion if it implements <see cref="ISoftDeletable"/>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public static void TryConfigureSoftDeletion(this EntityTypeBuilder builder)
    {
        if (builder.Metadata.ClrType.IsAssignableTo<ISoftDeletable>())
        {
            var fieldName = nameof(ISoftDeletable.IsDeleted);
            var parameter = Expression.Parameter(builder.Metadata.ClrType, "e");
            var property = Expression.Property(parameter, fieldName);
            var body = Expression.Equal(property, Expression.Constant(false));
            builder.Property(fieldName).IsRequired();
            builder.HasQueryFilter(Expression.Lambda(body, parameter));
        }
    }

    /// <summary>
    /// Configures the entity type to support concurrency stamps if it implements <see cref="IHasConcurrencyStamp"/>.
    /// </summary>
    /// <param name="builder">The entity type builder.</param>
    public static void TryConfigureConcurrencyStamp(this EntityTypeBuilder builder)
    {
        if (builder.Metadata.ClrType.IsAssignableTo<IHasConcurrencyStamp>())
        {
            var fieldName = nameof(IHasConcurrencyStamp.ConcurrencyStamp);
            builder.Property(fieldName).IsConcurrencyToken().HasMaxLength(36);
        }
    }

    /// <summary>
    /// Applies the default conventions to all entity types in the model.
    /// </summary>
    /// <param name="modelBuilder">The model builder.</param>
    public static void ApplyDefaultConventions(this ModelBuilder modelBuilder)
    {
        modelBuilder.Model.GetEntityTypes()
            .ForEach(mutableEntityType =>
            {
                if (!mutableEntityType.IsOwned())
                {
                    var entityBuilder = modelBuilder.Entity(mutableEntityType.ClrType);
                    entityBuilder.TryConfigureSoftDeletion();
                    entityBuilder.TryConfigureConcurrencyStamp();
                }
            });
    }
}
