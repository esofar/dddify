# Localization

Dddify provides JSON file-based localization services and integrates with the standard .NET localization interfaces. This module can be used for regular text localization, and can also provide error message resolution for business exceptions and API result wrapping.

## Registration

Enable the localization module in `AddDddify`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddLocalization();
});
```

This module registers the following services and configuration:

- `IStringLocalizerFactory`
- `IStringLocalizer<T>`
- `IStringLocalizer`
- `IConfigureOptions<RequestLocalizationOptions>`

In ASP.NET Core applications, if requests need to set `CurrentCulture` and `CurrentUICulture`, enable the localization middleware in the request pipeline:

```csharp
app.UseRequestLocalization();
```

## Configuration

`AddLocalization()` binds options from the `Localization` configuration section of `IConfiguration` by default. The following example shows a common `appsettings.json` configuration:

```json [appsettings.json]
{
  "Localization": {
    "ResourcesPath": "Resources",
    "SupportedCultures": [ "en-US", "zh-CN" ],
    "DefaultCulture": "zh-CN"
  }
}
```

Specify another configuration section when needed:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddLocalization("MyLocalization");
});
```

You can also override or supplement configuration values in code when registering the module:

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

`JsonLocalizationOptions` supports the following options:

| Option | Default | Description |
| --- | --- | --- |
| `ResourcesPath` | `Resources` | Resource file directory, relative to the application output directory. |
| `SupportedCultures` | `[]` | Supported culture list. When set, it is applied to both `SupportedCultures` and `SupportedUICultures`. |
| `DefaultCulture` | `null` | Default culture. When not set, the first value in `SupportedCultures` is used. |

When `SupportedCultures` is empty, Dddify does not modify the supported cultures or default culture of `RequestLocalizationOptions`. The current culture is then determined by the application itself or by the runtime environment.

## Resource Files

Resource files store key-value pairs as JSON objects. The path rule is `{ResourcesPath}/{culture}/{resourceTypeName}.json`. `resourceTypeName` comes from the resource type name of `IStringLocalizer<T>`; non-generic `IStringLocalizer` uses the built-in `SharedResource`, corresponding to `SharedResource.json`.

```text
Resources/
  en-US/
    SharedResource.json
    OrderResource.json
  zh-CN/
    SharedResource.json
    OrderResource.json
```

Example:

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

Resource files must be published to the application output directory. Set resource files as content files in the project file:

```xml
<ItemGroup>
  <Content Include="Resources\**\*.json" CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

## Using IStringLocalizer {#using-localizer}

Services can inject `IStringLocalizer` or `IStringLocalizer<T>` directly to read localized text. Dddify selects resource files by the current `CurrentUICulture`; when a resource does not exist, the resource key itself is returned.

For text with a clear business boundary, use `IStringLocalizer<T>`. `T` represents the resource type and is usually named by aggregate root, module, or business boundary. For example, an order aggregate can define `OrderResource` and read `OrderResource.json` under each culture directory through `IStringLocalizer<OrderResource>`.

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

Non-generic `IStringLocalizer` uses the built-in `SharedResource`. It is suitable for common error messages, system notification templates, and cross-module shared text.

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

## Business Exception Localization

Application exceptions and domain exceptions both inherit from `BusinessException`. Business exceptions maintain only a stable error contract:

- `ErrorCode`: stable error code, also used as the localization resource key.
- `ErrorArgs`: formatting arguments for localized messages.

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

Resource files use `ErrorCode` as the key. `ErrorArgs` are formatted with placeholders such as `{0}` and `{1}`:

```json
{
  "order_not_found": "Order {0} does not exist."
}
```

After `AddApiResultWrapping()` is enabled, the API result wrapping module resolves localized messages when handling business exceptions. Business exceptions use `SharedResource` by default. When resource files need to be split by aggregate, module, or business boundary, configure `IBusinessExceptionResourceTypeResolver`. See [API Result Wrapping: Exception Mapping](./api-result-wrapping.md#exception-mapping).

Framework-level error codes also use `SharedResource`. To localize these error messages, add the corresponding keys to `SharedResource.json` yourself:

::: code-group

```json [Resources/en-US/SharedResource.json]
{
  "validation_failed": "Validation failed.",
  "concurrency_conflict": "The data has been changed. Please reload and try again.",
  "internal_server_error": "An unexpected server error occurred."
}
```

:::

## Recommendations

- Error codes should remain stable and should not change when display text changes.
- Business resource files are grouped by the project. They can be maintained by aggregate, module, or business boundary; cross-module shared text belongs in `SharedResource.json`.
- The same error code should exist consistently across culture resource files to avoid missing keys in some cultures.
- Formatting arguments passed to `WithErrorCode(...)` are only for message display; resource text should avoid depending on complex object formatting.
- Resource files should not store sensitive information such as passwords, tokens, or connection strings.
