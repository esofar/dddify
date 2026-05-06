# 框架配置

`AddDddify()` 是 Dddify 的统一注册入口。它注册核心服务，并根据配置启用可选模块。

```csharp
builder.Services.AddDddify(cfg =>
{
    // 配置 Dddify 模块和扩展点
});
```

## 核心注册

`AddDddify()` 会注册 Dddify 的基础能力，包括类型扫描、处理器注册、验证器注册、映射配置和约定式依赖注入。

默认注册内容：

- MediatR：扫描命令、查询和领域事件处理器。
- FluentValidation：扫描并注册验证器。
- Mapster：扫描映射配置，并注册 `TypeAdapterConfig` 和 `IMapper`。
- Scrutor：按约定注册领域服务、仓储实现和带有依赖标记的服务。

默认管道行为：

- `ValidationBehavior<,>`：执行当前请求类型对应的 FluentValidation 验证器。
- `UnitOfWorkBehavior<,>`：围绕命令请求执行工作单元逻辑。

通常不建议关闭默认管道行为。仅当项目需要完全自行控制验证流程、保存时机或事务边界时，才应关闭对应管道行为：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.DisableValidationBehavior();
    cfg.DisableUnitOfWorkBehavior();
});
```

禁用后，对应行为不会加入 MediatR 管道，项目需要自行保证验证、保存和事务处理的一致性。

## 扫描程序集

默认情况下，Dddify 通过 `DependencyContext.Default` 获取当前应用引用的项目程序集，并用于 Mapster、MediatR、FluentValidation 和 Scrutor 扫描。需要精确控制扫描范围时，可以显式指定程序集：

```csharp
cfg.ScanAssemblies(
    typeof(ApplicationMarker).Assembly,
    typeof(DomainAssemblyMarker).Assembly,
    typeof(InfrastructureMarker).Assembly,
    typeof(Program).Assembly);
```

显式指定后，Dddify 只扫描传入的程序集。若某个处理器、验证器、映射配置或约定注册服务不在扫描范围内，该类型不会被自动注册。

## 可选模块

Dddify 核心注册只包含通用管道和约定式服务注册。时间、当前用户、本地化、API 结果包装和 EF Core 集成等能力需要按项目需求显式启用。

可选模块通过 `AddDddify` 的配置委托注册：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();
    cfg.AddCurrentUser();
    cfg.AddApiResultWrapping();
});
```

常用模块包括：

- [`AddTiming(...)`](/modules/timing)：需要统一读取当前时间、当前时区或进行时间转换。
- [`AddCurrentUser(...)`](/modules/current-user)：需要在应用层或基础设施层读取当前用户信息。
- [`AddLocalization(...)`](/modules/localization)：需要 JSON 本地化或错误消息本地化。
- [`AddApiResultWrapping(...)`](/modules/api-result-wrapping)：需要统一 API 响应结构和异常响应映射。
- [`AddDbContextWithUnitOfWork<TContext>(...)`](/modules/entity-framework-core)：使用 EF Core 提供数据持久化集成。

可选模块可独立启用，也可组合使用。各模块的配置项和使用方式请参考对应模块文档。

## 扩展配置

Dddify 提供 MediatR、Scrutor 和 Mapster 的配置扩展点。可在保留默认注册约定的基础上补充项目级配置：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.ConfigureMediatR(options =>
    {
        // 配置 MediatRServiceConfiguration
    });

    cfg.ConfigureScrutor(selector =>
    {
        // 添加额外的 Scrutor 扫描规则
    });

    cfg.ConfigureMapster(config =>
    {
        // 自定义 TypeAdapterConfig
    });
});
```

扩展配置会与 Dddify 的默认配置共同生效。若添加自定义扫描规则，应避免与已有注册产生不必要的重复注册。
