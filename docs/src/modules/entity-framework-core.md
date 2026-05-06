# 数据持久化

Dddify 通过 EF Core 提供数据持久化集成，用于注册 `DbContext`、协调工作单元、应用模型约定，并在保存前处理审计、软删除、并发标记和领域事件。

## 注册 DbContext

在 `AddDddify` 中使用 `AddDbContextWithUnitOfWork(...)` 注册并启用工作单元集成：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});
```

该扩展会注册：

- `DbContext`。
- `IUnitOfWork`，默认实现为 `UnitOfWork<TDbContext>`。
- `ApplyEntityStateInterceptor`。
- `DispatchDomainEventsInterceptor`。

`AddDbContextWithUnitOfWork(...)` 支持直接注册实现类型，也支持按服务类型和实现类型注册：

```csharp
cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>();
cfg.AddDbContextWithUnitOfWork<IApplicationDbContext, ApplicationDbContext>();
```

第二种写法适用于应用层依赖 `DbContext` 抽象服务类型的项目。

## 模型约定

`ApplyDefaultConventions()` 会遍历非 owned entity，并按接口应用默认模型配置：

- 实现 `ISoftDeletable` 时，配置 `IsDeleted` 为必填，并添加软删除查询过滤器。
- 实现 `IHasConcurrencyStamp` 时，将 `ConcurrencyStamp` 配置为并发标记。

在 `DbContext` 中使用：

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    modelBuilder.ApplyDefaultConventions();

    base.OnModelCreating(modelBuilder);
}
```

如果实体配置中也调用了 `HasQueryFilter(...)`，应注意 EF Core 查询过滤器的组合行为，避免覆盖软删除过滤器。

对于继承 `Enumeration` 的枚举类属性，可以使用 `HasEnumerationConversion()` 将属性按枚举值持久化：

```csharp
builder.Property(x => x.Priority)
    .HasEnumerationConversion();
```

## 工作单元

`AddDbContextWithUnitOfWork(...)` 会注册 EF Core 版 `IUnitOfWork`，默认实现为 `UnitOfWork<TDbContext>`。

`IUnitOfWork` 用于封装当前 `DbContext` 的保存和事务操作：

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

命令处理器通常不需要直接调用 `IUnitOfWork`，默认命令管道会负责保存和事务处理，详见 [应用编排：工作单元管道](../guide/application-flow.md#work-unit-pipeline)。

以下场景通常需要手动使用 `IUnitOfWork`：

- 命令使用 `[SkipUnitOfWorkBehavior]` 跳过默认工作单元管道，需要自行持久化变更。
- 后台任务、定时任务或应用服务未经过命令管道。
- 批量处理需要分批保存，避免一次性跟踪过多实体。
- 需要显式控制事务边界。

如果只需要保存当前 `DbContext` 跟踪的变更，可以直接调用 `IUnitOfWork.SaveChangesAsync()`：

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

需要显式控制事务边界时，应先开启事务，再保存变更，最后提交事务：

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

如果当前已有事务，`CurrentTransaction` 不为空。需要复用外层事务时，应避免重复开启和提交事务。

## 仓储使用方式

Dddify 支持两种仓储使用方式：

- 在基础设施层实现 Repository，并注入具体的 `DbContext`。
- 在应用层直接依赖 `DbContext` 抽象，由 EF Core 直接承担仓储职责。

### 实现 Repository

需要为聚合提供独立访问入口时，可以继承 `RepositoryBase<TDbContext, TEntity, TKey>` 实现仓储。Repository 位于基础设施层，直接注入具体的 `DbContext`，并按业务需要补充查询方法：

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

`RepositoryBase` 实现 `IRepository<TEntity, TKey>`，提供以下常用方法：

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
> `AsQueryable()` 适用于列表筛选、排序、分页等相对查询组合场景。如果应用层大量依赖 `AsQueryable()`，应考虑直接使用 `DbContext` 抽象。

### 直接使用 DbContext 抽象

EF Core 的 `DbContext` 和 `DbSet` 本身已经实现工作单元和仓储模式的基础能力。对于查询逻辑较简单、无需额外仓储封装的项目，应用层可以直接依赖 `DbContext` 抽象。

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

## 保存拦截器

`AddDbContextWithUnitOfWork(...)` 会为 `DbContext` 添加 Dddify 保存拦截器。

`ApplyEntityStateInterceptor` 在保存前处理实体状态：

- 新增实体：填充 `IHasCreatedBy`、`IHasCreatedAt`，并刷新 `IHasConcurrencyStamp`。
- 修改实体：填充 `IHasModifiedBy`、`IHasModifiedAt`，并刷新 `IHasConcurrencyStamp`。
- 删除实体：如果实体实现 `ISoftDeletable`，则转换为软删除，并填充 `IHasDeletedBy`、`IHasDeletedAt`。

审计用户来自 `ICurrentUser`。审计时间来自 `IClock`。未注册相关服务时，对应审计值为空。

`DispatchDomainEventsInterceptor` 会在保存前发布当前 `DbContext` 中聚合根的领域事件，并在发布后清空事件集合。

## 查询辅助方法

`QueryableExtensions` 提供常用查询组合方法：

- `Paged(page, size)`：对查询应用分页，不立即执行查询。
- `ToPagedResultAsync(page, size, cancellationToken)`：执行查询并返回 `IPagedResult<T>`。
- `WhereIf(condition, predicate)`：条件满足时添加 `Where`。
- `IncludeIf(condition, path)`：条件满足时添加 `Include`。

示例：

```csharp
var result = await todoRepository
    .AsQueryable()
    .AsNoTracking()
    .WhereIf(!string.IsNullOrWhiteSpace(keyword), x => x.Title.Contains(keyword))
    .ToPagedResultAsync(page, size, cancellationToken);
```