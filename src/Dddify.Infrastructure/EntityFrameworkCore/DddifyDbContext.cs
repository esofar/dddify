using Dddify.Auditing;
using Dddify.Domain.Entities;
using Dddify.EntityFrameworkCore.Extensions;
using Dddify.Guids;
using Dddify.Security.Identity;
using Dddify.Timing;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace Dddify.EntityFrameworkCore;

public class DddifyDbContext : DbContext
{
    private readonly IClock _clock;
    private readonly IPublisher _publisher;
    private readonly ICurrentUser _currentUser;
    private readonly IGuidGenerator _guidGenerator;

    private static readonly MethodInfo? ConfigureBasePropertiesMethodInfo
     = typeof(DddifyDbContext)
         .GetMethod(
             nameof(ConfigureBaseProperties),
             BindingFlags.Instance | BindingFlags.NonPublic
         );

    public DddifyDbContext(DbContextOptions options, IDbContextDependencies dependencies)
        : base(options)
    {
        _clock = dependencies.Clock;
        _publisher = dependencies.Publisher;
        _currentUser = dependencies.CurrentUser;
        _guidGenerator = dependencies.GuidGenerator;
    }

    public virtual async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        ApplyConcepts();

        var domainEvents = ChangeTracker
            .Entries<Entity>()
            .Where(c => c.Entity.DomainEvents != null && c.Entity.DomainEvents.Any())
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }

        return await base.SaveChangesAsync(cancellationToken);
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

    protected virtual void ConfigureBaseProperties<TEntity>(ModelBuilder modelBuilder, IMutableEntityType mutableEntityType) where TEntity : class
    {
        if (!mutableEntityType.IsOwned())
        {
            modelBuilder.Entity<TEntity>().ConfigureByConvention();
        }
    }

    protected virtual void ApplyConcepts()
    {
        foreach (var entry in ChangeTracker.Entries<IEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    TrySetGuidId(entry);
                    TrySetCreationAuditProperties(entry);
                    TrySetConcurrencyStampIfNull(entry);
                    break;
                case EntityState.Modified:
                    TrySetModificationAuditProperties(entry);
                    TryUpdateConcurrencyStamp(entry);
                    break;
                case EntityState.Deleted:
                    TrySetSoftDeletionProperties(entry);
                    TryUpdateConcurrencyStamp(entry);
                    break;
            }
        }
    }

    protected virtual void TrySetCreationAuditProperties(EntityEntry entry)
    {
        if (entry.Entity is ICreationAudited entity)
        {
            entity.CreatedBy = _currentUser.Id;
            entity.CreatedAt = _clock.Now;
        }
    }

    protected virtual void TrySetModificationAuditProperties(EntityEntry entry)
    {
        if (entry.Entity is IModificationAudited entity)
        {
            entity.LastModifiedBy = _currentUser.Id;
            entity.LastModifiedAt = _clock.Now;
        }
    }

    protected virtual void TrySetSoftDeletionProperties(EntityEntry entry)
    {
        if (entry.Entity is ISoftDeletable entity)
        {
            entry.Reload();
            entry.State = EntityState.Modified;
            entity.IsDeleted = true;

            TrySetModificationAuditProperties(entry);
        }
    }

    protected virtual void TrySetConcurrencyStampIfNull(EntityEntry entry)
    {
        if (entry.Entity is IHasConcurrencyStamp entity)
        {
            if (string.IsNullOrEmpty(entity.ConcurrencyStamp))
            {
                entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
            }
        }
    }

    protected virtual void TryUpdateConcurrencyStamp(EntityEntry entry)
    {
        if (entry.Entity is IHasConcurrencyStamp entity)
        {
            Entry(entity).Property(x => x.ConcurrencyStamp).OriginalValue = entity.ConcurrencyStamp;
            entity.ConcurrencyStamp = Guid.NewGuid().ToString("N");
        }
    }

    protected virtual void TrySetGuidId(EntityEntry entry)
    {
        if (entry.Entity is IEntity<Guid> entity)
        {
            if (entity.Id != default)
            {
                return;
            }

            entity.Id = _guidGenerator.Create();
        }
    }
}
