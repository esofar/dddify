# Data Persistence

Dddify provides data persistence integration through EF Core. It registers `DbContext`, coordinates unit of work, applies model conventions, and handles auditing, soft delete, concurrency tokens, and domain events before saving.

## Register DbContext

Use `AddDbContextWithUnitOfWork(...)` in `AddDddify` to register the `DbContext` and enable unit-of-work integration:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});
```

This extension registers:

- `DbContext`.
- `IUnitOfWork`, with `UnitOfWork<TDbContext>` as the default implementation.
- `ApplyEntityStateInterceptor`.
- `DispatchDomainEventsInterceptor`.

`AddDbContextWithUnitOfWork(...)` supports registering the implementation type directly, and also supports registering by service type and implementation type:

```csharp
cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>();
cfg.AddDbContextWithUnitOfWork<IApplicationDbContext, ApplicationDbContext>();
```

The second form is suitable for projects where the application layer depends on an abstract `DbContext` service type.

## Model Conventions

`ApplyDefaultConventions()` traverses non-owned entities and applies default model configuration by interface:

- When an entity implements `ISoftDeletable`, `IsDeleted` is configured as required and a soft-delete query filter is added.
- When an entity implements `IHasConcurrencyStamp`, `ConcurrencyStamp` is configured as a concurrency token.

Use it in `DbContext`:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    modelBuilder.ApplyDefaultConventions();

    base.OnModelCreating(modelBuilder);
}
```

If entity configuration also calls `HasQueryFilter(...)`, pay attention to EF Core query filter composition to avoid overriding the soft-delete filter.

For properties that inherit from `Enumeration`, use `HasEnumerationConversion()` to persist the property by enumeration value:

```csharp
builder.Property(x => x.Priority)
    .HasEnumerationConversion();
```

## Unit of Work

`AddDbContextWithUnitOfWork(...)` registers the EF Core implementation of `IUnitOfWork`, with `UnitOfWork<TDbContext>` as the default implementation.

`IUnitOfWork` encapsulates saving and transaction operations for the current `DbContext`:

```csharp
public interface IUnitOfWork
{
    IDbContextTransaction? CurrentTransaction { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
```

Command handlers usually do not need to call `IUnitOfWork` directly. The default command pipeline handles saving and transaction processing. See [Application Flow: Unit of Work Pipeline](../guide/application-flow.md#work-unit-pipeline).

The following scenarios usually require manual use of `IUnitOfWork`:

- A command uses `[SkipUnitOfWorkBehavior]` to skip the default unit-of-work pipeline and must persist changes manually.
- Background jobs, scheduled tasks, or application services do not go through the command pipeline.
- Batch processing needs to save in batches to avoid tracking too many entities at once.
- Transaction boundaries must be controlled explicitly.

If you only need to save changes tracked by the current `DbContext`, call `IUnitOfWork.SaveChangesAsync()` directly:

```csharp
[SkipUnitOfWorkBehavior]
public record ImportTodosCommand(IEnumerable<Todo> Todos) : ICommand;

public class ImportTodosCommandHandler(
    ITodoRepository todoRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ImportTodosCommand>
{
    public async Task Handle(ImportTodosCommand command, CancellationToken cancellationToken)
    {
        await todoRepository.AddRangeAsync(command.Todos, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
```

When transaction boundaries must be controlled explicitly, start the transaction first, save changes, and then commit:

```csharp
[SkipUnitOfWorkBehavior]
public record ImportTodosCommand(IEnumerable<Todo> Todos) : ICommand;

public class ImportTodosCommandHandler(
    ITodoRepository todoRepository,
    IUnitOfWork unitOfWork)
    : ICommandHandler<ImportTodosCommand>
{
    public async Task Handle(ImportTodosCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            await todoRepository.AddRangeAsync(command.Todos, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
```

If a transaction already exists, `CurrentTransaction` is not null. When reusing an outer transaction, avoid starting and committing a transaction repeatedly.

## Repository Usage

Dddify supports two repository usage styles:

- Implement Repository in the infrastructure layer and inject the concrete `DbContext`.
- Depend directly on an abstract `DbContext` in the application layer and let EF Core provide repository responsibilities.

### Implement Repository

When an aggregate needs an independent access entry point, inherit from `RepositoryBase<TDbContext, TEntity, TKey>` to implement a repository. Repository implementations live in the infrastructure layer, inject the concrete `DbContext` directly, and add query methods according to business needs:

::: code-group

```csharp [ITodoRepository.cs]
public interface ITodoRepository : IRepository<Todo, Guid>
{
    Task<Todo?> FindByTitleAsync(string title, CancellationToken cancellationToken = default);
}
```

```csharp [TodoRepository.cs]
public sealed class TodoRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, Todo, Guid>(context), ITodoRepository
{
    public Task<Todo?> FindByTitleAsync(string title, CancellationToken cancellationToken = default)
    {
        return AsQueryable()
            .FirstOrDefaultAsync(x => x.Title == title, cancellationToken);
    }
}
```

```csharp [Program.cs]
builder.Services.AddDddify(cfg =>
{
    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});
```

```csharp [CreateTodoCommandHandler.cs]
public sealed class CreateTodoCommandHandler(ITodoRepository todoRepository)
    : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        if (await todoRepository.FindByTitleAsync(command.Title, cancellationToken) is not null)
        {
            throw new TodoTitleDuplicateException(command.Title);
        }

        var todo = new Todo(Guid.NewGuid(), command.Title);

        await todoRepository.AddAsync(todo, cancellationToken);

        return todo.Id;
    }
}
```

:::

`RepositoryBase` implements `IRepository<TEntity, TKey>` and provides these common methods:

- `AsQueryable()`
- `GetAsync(...)`
- `GetAllAsync(...)`
- `GetListAsync(...)`
- `AnyAsync(...)`
- `AddAsync(...)` / `AddRangeAsync(...)`
- `Update(...)` / `UpdateRange(...)`
- `Remove(...)` / `RemoveRange(...)`
- `SetOriginalConcurrencyStamp(...)`

> [!NOTE]
> `AsQueryable()` is suitable for relatively simple query composition scenarios such as list filtering, sorting, and paging. If the application layer relies heavily on `AsQueryable()`, consider using a `DbContext` abstraction directly.

### Use a DbContext Abstraction Directly

EF Core's `DbContext` and `DbSet` already implement the basic capabilities of the unit-of-work and repository patterns. For projects with simple query logic and no need for additional repository encapsulation, the application layer can depend directly on a `DbContext` abstraction.

::: code-group

```csharp [IApplicationDbContext.cs]
public interface IApplicationDbContext
{
    DbSet<Todo> Todos { get; }

    void SetOriginalConcurrencyStamp<TEntity>(TEntity entity, string? concurrencyStamp)
        where TEntity : class, IHasConcurrencyStamp;
}
```

```csharp [ApplicationDbContext.cs]
public class ApplicationDbContext(DbContextOptions options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Todo> Todos => Set<Todo>();

    public void SetOriginalConcurrencyStamp<TEntity>(TEntity entity, string? concurrencyStamp)
        where TEntity : class, IHasConcurrencyStamp
    {
        Entry(entity)
            .Property(nameof(IHasConcurrencyStamp.ConcurrencyStamp))
            .OriginalValue = concurrencyStamp;
    }
}
```

```csharp [Program.cs]
builder.Services.AddDddify(cfg =>
{
    cfg.AddDbContextWithUnitOfWork<IApplicationDbContext, ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});
```

```csharp [GetTodoByIdQueryHandler.cs]
public sealed class GetTodoByIdQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetTodoByIdQuery, TodoDto>
{
    public async Task<TodoDto> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        var todo = await context.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == query.Id, cancellationToken)
            ?? throw new TodoNotFoundException(query.Id);

        return new TodoDto(todo.Id, todo.Title);
    }
}
```

:::

## Save Interceptors

`AddDbContextWithUnitOfWork(...)` adds Dddify save interceptors to the `DbContext`.

`ApplyEntityStateInterceptor` handles entity state before saving:

- Added entity: fills `IHasCreatedBy` and `IHasCreatedAt`, and refreshes `IHasConcurrencyStamp`.
- Modified entity: fills `IHasModifiedBy` and `IHasModifiedAt`, and refreshes `IHasConcurrencyStamp`.
- Deleted entity: if the entity implements `ISoftDeletable`, converts it to soft delete and fills `IHasDeletedBy` and `IHasDeletedAt`.

The auditing user comes from `ICurrentUser`. The auditing time comes from `IClock`. If the related services are not registered, the corresponding auditing values are empty.

`DispatchDomainEventsInterceptor` publishes domain events from aggregate roots in the current `DbContext` before saving and clears the event collection after publishing.

## Query Helper Methods

`QueryableExtensions` provides common query composition methods:

- `Paged(page, size)`: applies paging to a query without executing it immediately.
- `ToPagedResultAsync(page, size, cancellationToken)`: executes the query and returns `IPagedResult<T>`.
- `WhereIf(condition, predicate)`: adds `Where` when the condition is true.
- `IncludeIf(condition, path)`: adds `Include` when the condition is true.

Example:

```csharp
var result = await todoRepository
    .AsQueryable()
    .AsNoTracking()
    .WhereIf(!string.IsNullOrWhiteSpace(keyword), x => x.Title.Contains(keyword))
    .ToPagedResultAsync(page, size, cancellationToken);
```
