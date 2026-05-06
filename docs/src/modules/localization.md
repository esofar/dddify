# 本地化

Dddify 提供基于 JSON 文件的本地化服务，并接入 .NET 标准本地化接口。该模块可用于普通文本本地化，也可为业务异常和 API 结果包装提供错误消息解析。

## 注册

在 `AddDddify` 中启用本地化模块：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddLocalization();
});
```

该模块会注册以下服务和配置：

- `IStringLocalizerFactory`
- `IStringLocalizer<T>`
- `IStringLocalizer`
- `IConfigureOptions<RequestLocalizationOptions>`

在 ASP.NET Core 应用中，如需根据请求设置 `CurrentCulture` 和 `CurrentUICulture`，还应在请求管道中启用本地化中间件：

```csharp
app.UseRequestLocalization();
```

## 配置

`AddLocalization()` 默认从 `IConfiguration` 的 `Localization` 配置节绑定选项。以下示例展示了通过 `appsettings.json` 提供配置的常见写法：

```json [appsettings.json]
{
  "Localization": {
    "ResourcesPath": "Resources",
    "SupportedCultures": [ "en-US", "zh-CN" ],
    "DefaultCulture": "zh-CN"
  }
}
```

需要使用其它配置节时，可以显式指定：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddLocalization("MyLocalization");
});
```

也可以在注册模块时通过代码覆盖或补充配置节中的值：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddLocalization(options =>
    {
        options.ResourcesPath = "Resources";
        options.SupportedCultures = ["en-US", "zh-CN"];
        options.DefaultCulture = "zh-CN";
    });
});
```

`JsonLocalizationOptions` 支持以下选项：

| 配置项 | 默认值 | 说明 |
| --- | --- | --- |
| `ResourcesPath` | `Resources` | 资源文件目录，相对于应用程序输出目录。 |
| `SupportedCultures` | `[]` | 支持的区域性列表；设置后会同时应用到 `SupportedCultures` 和 `SupportedUICultures`。 |
| `DefaultCulture` | `null` | 默认区域性；未设置时使用 `SupportedCultures` 的第一个值。 |

当 `SupportedCultures` 为空时，Dddify 不会修改 `RequestLocalizationOptions` 的支持区域性和默认区域性。此时当前区域性由应用自身或运行时环境决定。

## 资源文件

资源文件使用 JSON 对象保存键值对，路径规则为 `{ResourcesPath}/{culture}/{resourceTypeName}.json`。`resourceTypeName` 来自 `IStringLocalizer<T>` 的资源类型名；非泛型 `IStringLocalizer` 使用内置的 `SharedResource`，对应 `SharedResource.json`。

```text
Resources/
  en-US/
    SharedResource.json
    OrderResource.json
  zh-CN/
    SharedResource.json
    OrderResource.json
```

示例：

::: code-group

```json [Resources/en-US/SharedResource.json]
{
  "validation_failed": "Validation failed."
}
```

```json [Resources/en-US/OrderResource.json]
{
  "order_not_found": "Order {0} does not exist."
}
```

:::

资源文件必须随应用发布到输出目录。可以在项目文件中将资源文件设置为内容文件：

```xml
<ItemGroup>
  <Content Include="Resources\**\*.json" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

## 使用 IStringLocalizer {#using-localizer}

各类服务可以直接注入 `IStringLocalizer` 或 `IStringLocalizer<T>` 读取本地化文本。Dddify 按当前 `CurrentUICulture` 选择资源文件；资源不存在时返回资源键本身。

业务边界明确的文本建议使用 `IStringLocalizer<T>`。`T` 表示资源类型，通常按聚合根、模块或业务边界命名。例如订单聚合可以定义 `OrderResource`，并通过 `IStringLocalizer<OrderResource>` 读取各区域性目录下的 `OrderResource.json`。

::: code-group

```csharp [OrderPaidDomainEventHandler.cs]
public record OrderPaidDomainEvent(Guid OrderId, string OrderNumber, string Email) : IDomainEvent;

public class OrderPaidDomainEventHandler(
    IStringLocalizer<OrderResource> localizer,
    IEmailSender emailSender,
    ILogger<OrderPaidDomainEventHandler> logger)
    : IDomainEventHandler<OrderPaidDomainEvent>
{
    public async Task Handle(OrderPaidDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = localizer["order_paid_subject"];
        var body = localizer["order_paid_body", @event.OrderNumber];

        await emailSender.SendAsync(@event.Email, subject, body, cancellationToken);

        logger.LogInformation(
            "Order paid notification sent. OrderId: {OrderId}, OrderNumber: {OrderNumber}",
            @event.OrderId,
            @event.OrderNumber);
    }
}
```

```csharp [OrderResource.cs]
public class OrderResource
{
}
```

```json [Resources/en-US/OrderResource.json]
{
  "order_paid_subject": "Order paid",
  "order_paid_body": "Order {0} has been paid."
}
```

:::

非泛型 `IStringLocalizer` 使用内置的 `SharedResource`，适合读取通用错误消息、系统通知模板和跨模块共享文本。

::: code-group

```csharp [PasswordResetSucceededDomainEventHandler.cs]
public record PasswordResetSucceededDomainEvent(Guid UserId, string Email) : IDomainEvent;

public class PasswordResetSucceededDomainEventHandler(
    IStringLocalizer localizer,
    IEmailSender emailSender,
    ILogger<PasswordResetSucceededDomainEventHandler> logger)
    : IDomainEventHandler<PasswordResetSucceededDomainEvent>
{
    public async Task Handle(PasswordResetSucceededDomainEvent @event, CancellationToken cancellationToken)
    {
        var subject = localizer["password_reset_succeeded_subject"];
        var body = localizer["password_reset_succeeded_body"];

        await emailSender.SendAsync(@event.Email, subject, body, cancellationToken);

        logger.LogInformation(
            "Password reset succeeded notification sent. UserId: {UserId}",
            @event.UserId);
    }
}
```

```json [Resources/en-US/SharedResource.json]
{
  "password_reset_succeeded_subject": "Your password has been reset",
  "password_reset_succeeded_body": "Your password was reset successfully. If this was not you, contact support."
}
```

:::

## 业务异常本地化

应用异常和领域异常都继承自 `BusinessException`。业务异常只维护稳定错误契约：

- `ErrorCode`：稳定错误码，同时作为本地化资源 key。
- `ErrorArgs`：本地化消息格式化参数。

```csharp
public class OrderNotFoundException : AppException
{
    public OrderNotFoundException(Guid orderId)
    {
        WithErrorCode("order_not_found", orderId);
        WithMetadata("OrderId", orderId);
    }
}
```

资源文件使用 `ErrorCode` 作为 key；`ErrorArgs` 按 `{0}`、`{1}` 等占位符格式化：

```json
{
  "order_not_found": "Order {0} does not exist."
}
```

启用 `AddApiResultWrapping()` 后，API 结果包装模块会在处理业务异常时解析本地化消息。业务异常默认使用 `SharedResource`；需要按聚合、模块或业务边界拆分资源文件时，可配置 `IBusinessExceptionResourceTypeResolver`，见 [API 结果包装：异常映射](./api-result-wrapping.md#exception-mapping)。

框架级错误码也使用 `SharedResource` 解析。需要本地化这些错误消息时，应在 `SharedResource.json` 中自行添加对应 key：

::: code-group

```json [Resources/en-US/SharedResource.json]
{
  "validation_failed": "Validation failed.",
  "concurrency_conflict": "The data has been changed. Please reload and try again.",
  "internal_server_error": "An unexpected server error occurred."
}
```

:::

## 使用建议

- 错误码应保持稳定，避免随显示文案调整而变更。
- 业务资源文件由项目自行分组，可按聚合、模块或业务边界维护；跨模块通用文本放入 `SharedResource.json`。
- 同一个错误码在不同区域性资源文件中应保持一致，避免部分区域性缺失 key。
- `WithErrorCode(...)` 的格式化参数只用于消息展示；资源文本应避免依赖复杂对象格式。
- 资源文件不应保存密码、令牌、连接字符串等敏感信息。