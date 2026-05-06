using Dddify.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Linq.Expressions;

namespace Dddify.EntityFrameworkCore;

/// <summary>
/// Provides EF Core builder extensions for applying Dddify persistence conventions.
/// </summary>
/// <remarks>
/// These helpers bridge domain-level contracts such as soft deletion, concurrency stamps,
/// and enumeration classes into EF Core model configuration.
/// </remarks>
public static class BuilderExtensions
{
    /// <summary>
    /// Applies soft-delete configuration when the entity type implements <see cref="ISoftDeletable"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public static void TryConfigureSoftDeletion(this EntityTypeBuilder builder)
    {
        if (builder.Metadata.ClrType.IsAssignableTo(typeof(ISoftDeletable)))
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
    /// Applies optimistic concurrency configuration when the entity type implements <see cref="IHasConcurrencyStamp"/>.
    /// </summary>
    /// <param name="builder">The EF Core entity type builder.</param>
    public static void TryConfigureConcurrencyStamp(this EntityTypeBuilder builder)
    {
        if (builder.Metadata.ClrType.IsAssignableTo(typeof(IHasConcurrencyStamp)))
        {
            var fieldName = nameof(IHasConcurrencyStamp.ConcurrencyStamp);
            builder.Property(fieldName).IsConcurrencyToken().HasMaxLength(36);
        }
    }

    /// <summary>
    /// Applies Dddify's default persistence conventions to all non-owned entity types in the model.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    public static void ApplyDefaultConventions(this ModelBuilder modelBuilder)
    {
        var mutableEntityTypes = modelBuilder.Model.GetEntityTypes();
        foreach (var mutableEntityType in mutableEntityTypes)
        {
            if (!mutableEntityType.IsOwned())
            {
                var entityBuilder = modelBuilder.Entity(mutableEntityType.ClrType);
                entityBuilder.TryConfigureSoftDeletion();
                entityBuilder.TryConfigureConcurrencyStamp();
            }
        }
    }


    /// <summary>
    /// Configures an enumeration-class property to be stored by its numeric value.
    /// </summary>
    /// <remarks>
    /// Use this extension for domain types derived from <see cref="Enumeration"/> when the value
    /// should be persisted as the enumeration identifier rather than as a complex object.
    /// </remarks>
    /// <typeparam name="TEnumeration">The enumeration-class type.</typeparam>
    /// <param name="propertyBuilder">The EF Core property builder.</param>
    /// <returns>The same property builder instance so configuration can continue fluently.</returns>
    public static PropertyBuilder<TEnumeration> HasEnumerationConversion<TEnumeration>(this PropertyBuilder<TEnumeration> propertyBuilder)
        where TEnumeration : Enumeration
    {
        return propertyBuilder.HasConversion(
            x => x.Id,
            x => Enumeration.FromValue<TEnumeration>(x));
    }
}
