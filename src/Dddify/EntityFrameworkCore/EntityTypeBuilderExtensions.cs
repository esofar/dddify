using Dddify.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dddify.EntityFrameworkCore;

public static class EntityTypeBuilderExtensions
{
    public static void ConfigureByConvention<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.TryConfigureSoftDeletion();
        builder.TryConfigureConcurrencyStamp();
    }

    public static void TryConfigureSoftDeletion<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        if (builder.Metadata.ClrType.IsAssignableTo<ISoftDeletable>())
        {
            var fieldName = nameof(ISoftDeletable.IsDeleted);
            builder.Property(fieldName).IsRequired();
            builder.HasQueryFilter(e => !EF.Property<bool>(e, fieldName));
        }
    }

    public static void TryConfigureConcurrencyStamp<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        if (builder.Metadata.ClrType.IsAssignableTo<IHasConcurrencyStamp>())
        {
            var fieldName = nameof(IHasConcurrencyStamp.ConcurrencyStamp);
            builder.Property(fieldName).IsConcurrencyToken().HasMaxLength(36);
        }
    }
}