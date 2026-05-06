# 依赖注入

Dddify 基于 ASP.NET Core 内置依赖注入容器，并使用 Scrutor 提供约定式注册。依赖注入只负责对象创建和依赖组装，不应承载业务分支、流程判断或领域规则。

## 使用原则

- 优先使用构造函数注入，使类型依赖保持显式。
- 优先依赖接口或稳定抽象，降低模块间耦合。
- 服务注册应集中管理，避免在启动代码中分散重复注册。
- 不要将 `Scoped` 服务注入 `Singleton` 服务。
- 同一服务多次注册时，最终解析结果遵循 ASP.NET Core DI 的注册顺序规则。
- 业务类型不应通过 `IServiceProvider` 动态查找核心依赖。

## 约定注册

调用 `AddDddify()` 后，Dddify 会扫描配置的程序集，并按以下约定注册服务：

- 实现 `IDomainService` 的类型：注册为自身类型，生命周期为 `Scoped`。
- 实现 `IRepository<TEntity, TKey>` 的仓储实现：按匹配接口注册，生命周期为 `Scoped`。
- 实现 `IValidator<T>` 的验证器：注册为实现的接口，生命周期为 `Scoped`。
- 标记了依赖注册特性的类型：按特性指定的生命周期和注册方式注册。

## 依赖标记

普通应用服务、基础服务或工具类可以使用依赖标记声明生命周期：

- `TransientDependencyAttribute`
- `ScopedDependencyAttribute`
- `SingletonDependencyAttribute`

每个标记都需要指定 `RegistrationMode`，用于控制服务暴露方式：

- `AsSelf`：仅注册为具体实现类型。
- `AsMatchingInterface`：注册为与类型名称匹配的接口。
- `AsImplementedInterfaces`：注册为该类型实现的所有接口。

当服务只在当前模块内部使用，不需要通过接口暴露时，可以使用 `AsSelf`：

```csharp
[TransientDependency(RegistrationMode.AsSelf)]
public class OrderPricingCalculator
{
    public decimal Calculate(decimal subtotal, decimal discount)
        => subtotal - discount;
}
```

当类型与接口遵循一一对应命名约定时，可以使用 `AsMatchingInterface`：

```csharp
public interface IOrderNumberGenerator
{
    string Create();
}

[ScopedDependency(RegistrationMode.AsMatchingInterface)]
public class OrderNumberGenerator : IOrderNumberGenerator
{
    public string Create() => $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}";
}
```

当一个实现需要同时暴露多个接口时，可以使用 `AsImplementedInterfaces`：

```csharp
public interface IOrderNotificationSender
{
    Task SendOrderCreatedAsync(Guid orderId, CancellationToken cancellationToken);
}

public interface IOrderNotificationHealthCheck
{
    bool IsAvailable();
}

[SingletonDependency(RegistrationMode.AsImplementedInterfaces)]
public class EmailOrderNotificationService : IOrderNotificationSender, IOrderNotificationHealthCheck
{
    public Task SendOrderCreatedAsync(Guid orderId, CancellationToken cancellationToken)
        => Task.CompletedTask;

    public bool IsAvailable() => true;
}
```

生命周期选择建议如下：

| 生命周期 | 适用场景 |
| --- | --- |
| `ScopedDependency(...)` | 依赖仓储、当前用户等请求级对象的应用服务或基础设施服务。 |
| `TransientDependency(...)` | 轻量、无状态、可重复创建的服务，例如格式化器、转换器。 |
| `SingletonDependency(...)` | 全局共享且线程安全的服务，例如配置提供器、静态规则缓存。 |

## 自定义扫描注册

当项目需要补充批量注册规则时，可以通过 `ConfigureScrutor(...)` 追加 Scrutor 配置。该方法暴露的是 Scrutor 的 `ITypeSourceSelector` 配置入口，适合按命名、接口、特性或程序集注册服务。

下面示例仅展示常见用法。具体扫描语法、注册方式、生命周期配置和重复注册策略，应以 [Scrutor 官方文档](https://github.com/khellang/Scrutor) 为准。

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.ConfigureScrutor(selector =>
    {
        selector
            .FromAssemblyOf<ApplicationMarker>()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Provider")))
            .AsImplementedInterfaces()
            .WithScopedLifetime();
    });
});
```

自定义规则会与 Dddify 默认规则共同生效。若同一服务可能被多条规则注册，应结合 Scrutor 和 ASP.NET Core DI 的注册规则，明确注册顺序和预期解析结果。

## 显式注册

并非所有服务都适合通过标记或扫描注册。以下场景建议使用 .NET 原生 DI API 手动注册：

- 第三方组件或外部 SDK。
- 需要从配置项创建的服务。
- 需要通过工厂方法创建的服务。
- 需要显式选择某个实现的服务。
- 不希望受约定扫描影响的特殊实现。

```csharp
builder.Services.AddSingleton<IOrderCodeFormatter, DefaultOrderCodeFormatter>();

builder.Services.AddScoped<IMessageSender>(sp =>
{
    var options = sp.GetRequiredService<IOptions<MessageOptions>>().Value;
    return new EmailMessageSender(options.ConnectionString);
});
```

手动注册建议放在 Web 项目的启动配置或 Infrastructure 层的服务扩展方法中，避免分散到业务类型中。

## 注入方式

业务代码应优先使用构造函数注入。依赖关系显式出现在类型签名中，调用方和测试代码可以直接识别类型需要的协作者。

```csharp
public class CreateOrderCommandHandler(
    IOrderRepository orderRepository,
    IOrderNumberGenerator orderNumberGenerator)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderNumber = orderNumberGenerator.Create();
        var order = new Order(Guid.NewGuid(), command.CustomerId, orderNumber);

        await orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
```

不建议在命令处理器、领域服务或应用服务中注入 `IServiceProvider` 再动态查找依赖。这会隐藏真实依赖，并可能引入生命周期问题。

`IServiceProvider` 更适合基础设施场景：

- 工厂对象按条件创建具体实现。
- 后台任务中手动创建服务作用域。
- 中间件或框架扩展点中访问请求级服务。

如果业务类型需要大量依赖，应优先检查职责边界是否过大，而不是使用 `IServiceProvider` 隐藏构造函数参数。

