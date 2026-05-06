# 当前用户

Dddify 提供当前用户抽象，用于在应用层和基础设施层读取当前执行上下文中的用户信息。

## 注册

在 `AddDddify` 中启用当前用户模块：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser();
});
```

`AddCurrentUser(...)` 会注册：

- `IHttpContextAccessor`
- `ICurrentUserProvider`
- `ICurrentUser`

默认 `ICurrentUserProvider` 从 `HttpContext.User` 读取当前 `ClaimsPrincipal`。非 HTTP 上下文中没有可用的 `HttpContext` 时，会返回空的 `ClaimsPrincipal`。

## ICurrentUser

`ICurrentUser` 是应用代码读取当前用户信息的主要入口。

```csharp
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string Id { get; }
    IReadOnlyCollection<Claim> Claims { get; }
    string? FindClaim(string claimType);
}
```

默认用户标识 claim type 为 `ClaimTypes.NameIdentifier`。未找到该 claim 时，`Id` 返回空字符串。`IsAuthenticated` 只有在身份已认证且 `Id` 非空时才返回 `true`。

示例：

```csharp
public class CreateOrderCommandHandler(
    ICurrentUser currentUser,
    IOrderRepository orderRepository,
    IGuidGenerator guidGenerator)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var buyerId = currentUser.GetIdAsGuid();
        var order = new Order(guidGenerator.Create(), buyerId, command.ShippingAddress);

        await orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
```

当前用户属于执行上下文信息。领域对象不应直接依赖 `ICurrentUser`；需要用户标识时，应由应用层读取后作为参数传入领域行为。

## 配置用户标识 Claim

使用 `UseIdClaim(...)` 可以指定解析用户标识的 claim type。OIDC/JWT 场景通常使用 `sub`：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser(options =>
    {
        options.UseIdClaim("sub");
    });
});
```

## 自定义 Provider

`ICurrentUserProvider` 负责为当前执行上下文提供 `ClaimsPrincipal`：

```csharp
public interface ICurrentUserProvider
{
    ClaimsPrincipal GetCurrentUser();
}
```

后台任务、消息消费、定时任务等非 HTTP 场景可以替换默认 provider。

```csharp
public class SystemCurrentUserProvider : ICurrentUserProvider
{
    public ClaimsPrincipal GetCurrentUser()
    {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "system")],
            authenticationType: "System");

        return new ClaimsPrincipal(identity);
    }
}
```

注册自定义 provider：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser(options =>
    {
        options.UseProvider<SystemCurrentUserProvider>();
    });
});
```

## 扩展当前用户

需要暴露租户、组织、权限标记等项目级信息时，可以定义继承 `ICurrentUser` 的接口，并通过 `Enhanced<TAbstraction, TImplementation>()` 注册额外服务。

```csharp
public interface IApplicationCurrentUser : ICurrentUser
{
    string? TenantId { get; }
}

public class ApplicationCurrentUser(ICurrentUser currentUser) : IApplicationCurrentUser
{
    public bool IsAuthenticated => currentUser.IsAuthenticated;

    public string Id => currentUser.Id;

    public IReadOnlyCollection<Claim> Claims => currentUser.Claims;

    public string? TenantId => FindClaim("tenant_id");

    public string? FindClaim(string claimType)
        => currentUser.FindClaim(claimType);
}
```

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser(options =>
    {
        options.Enhanced<IApplicationCurrentUser, ApplicationCurrentUser>();
    });
});
```

`Enhanced(...)` 会额外注册指定接口，不会替换默认的 `ICurrentUser`。

## 用户标识转换

`CurrentUserExtensions` 提供用户标识的类型化读取辅助方法：

```csharp
Guid userId = currentUser.GetIdAsGuid();
long longId = currentUser.GetIdAsLong();
int intId = currentUser.GetIdAsInt();
```

这些方法会在 `Id` 为空或格式不匹配时抛出 `InvalidOperationException`。匿名访问或系统任务场景应先判断 `IsAuthenticated`，或通过自定义 `ICurrentUserProvider` 提供明确的用户标识。


## 使用建议

- 在应用层读取当前用户，并将必要标识传入领域模型。
- 不在领域对象中注入 `ICurrentUser`、`IHttpContextAccessor` 或 `ClaimsPrincipal`。
- 后台任务和消息消费应使用自定义 `ICurrentUserProvider` 明确用户上下文。
- 用户 ID 类型应在项目内保持一致，避免在不同用例中混用 `Guid`、`long`、`int`。