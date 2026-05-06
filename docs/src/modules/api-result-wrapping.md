# 结果包装

Dddify 为 ASP.NET Core Web API 提供统一的 API 响应结构，用于约定成功响应、失败响应、字段级验证错误和追踪标识的输出格式。

## 注册

在 `AddDddify` 中启用 API 结果包装模块：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddApiResultWrapping();
});
```

`AddApiResultWrapping()` 会为带 `[ApiController]` 的 Web API action 启用：

- 已知 JSON/API 返回值包装。
- 未处理异常到统一失败响应的映射。

## 配置

`AddApiResultWrapping(...)` 可配置响应中的追踪标识、框架级错误码、异常 HTTP 状态码，以及业务异常的默认状态码和资源解析器。

`ApiResultWrappingOptions` 选项说明：

| 选项 | 默认值 | 说明 |
| --- | --- | --- |
| `EnableTraceIdentifier` | `true` | 是否在响应中输出 `traceId`。 |
| `ValidationException.ErrorCode` | `validation_failed` | 验证失败使用的错误码。 |
| `ValidationException.StatusCode` | `400` | 验证失败使用的 HTTP 状态码。 |
| `ConcurrencyException.ErrorCode` | `concurrency_conflict` | 并发冲突使用的错误码。 |
| `ConcurrencyException.StatusCode` | `409` | 并发冲突使用的 HTTP 状态码。 |
| `InternalServerException.ErrorCode` | `internal_server_error` | 未知异常使用的错误码。 |
| `InternalServerException.StatusCode` | `500` | 未知异常使用的 HTTP 状态码。 |
| `BusinessException.StatusCode` | `200` | 业务异常使用的默认 HTTP 状态码。 |
| `BusinessException.UseResourceTypeResolver` | `SharedResource` | 业务异常错误消息默认使用的资源类型。 |

## 响应结构

API 结果包装使用 `ApiResult`、`ApiResult<T>` 和 `ApiResultWithErrors` 表示统一响应结构。序列化后的 JSON 使用以下字段：

- `success`：表示请求是否成功。
- `data`：成功响应的数据，仅在有返回值时输出。
- `errorCode`：失败响应的稳定错误码。
- `errorMessage`：失败响应的错误消息，通常来自本地化资源或错误码回退。
- `errors`：字段级验证错误，结构为字段名到错误消息数组的映射。
- `traceId`：请求追踪标识。

成功响应示例：

```json
{
  "success": true,
  "data": {
    "id": "8a0f4f1e-1f8a-4b5d-9b9f-4ad7a5a2d111"
  },
  "traceId": "0HMS..."
}
```

失败响应示例：

```json
{
  "success": false,
  "errorCode": "order_not_found",
  "errorMessage": "Order does not exist.",
  "traceId": "0HMS..."
}
```

验证失败响应示例：

```json
{
  "success": false,
  "errorCode": "validation_failed",
  "errorMessage": "Validation failed.",
  "traceId": "0HMS...",
  "errors": {
    "Name": [ "Name is required." ]
  }
}
```

## 结果包装

结果包装仅作用于带 `[ApiController]` 的 action，并只处理已知 JSON/API 返回值。未识别的 `IActionResult` 保持原始结果，例如 `FileResult`、`RedirectResult` 和流式响应。

成功结果：

- `ObjectResult`、`OkObjectResult` 包装为 `ApiResult<T>`。
- `NoContentResult`、`EmptyResult` 和非错误 `StatusCodeResult` 包装为 `ApiResult`。

失败结果：

- `ValidationProblemDetails`、`SerializableError`、`ModelStateDictionary` 包装为 `ApiResultWithErrors`。
- `ProblemDetails` 包装为 `ApiResult`，并优先读取 `Extensions["code"]` 作为错误码。
- 4xx、5xx 的 `StatusCodeResult` 包装为失败 `ApiResult`。

如果接口需要返回约定的原始 JSON 结构，可以使用 `[DisableResultWrapping]` 禁用结果包装。该场景常见于第三方回调确认响应：

```csharp
[ApiController]
[Route("api/payment-webhooks")]
public class PaymentWebhooksController(IPaymentWebhookService paymentWebhookService) : ControllerBase
{
    [HttpPost("provider")]
    [DisableResultWrapping]
    public async Task<IActionResult> Provider(
        [FromBody] PaymentWebhook webhook,
        [FromHeader(Name = "X-Signature")] string signature,
        CancellationToken cancellationToken)
    {
        await paymentWebhookService.HandleAsync(webhook, signature, cancellationToken);

        return Ok(new { code = "SUCCESS", message = "OK" });
    }
}
```

## 异常映射 {#exception-mapping}

启用 API 结果包装后，action 中未处理的异常会被转换为失败响应。默认映射规则如下：

| 异常类型 | 处理方式 | 响应 |
| --- | --- | --- |
| `BusinessException` | 使用异常自身的 `ErrorCode`，状态码由业务异常选项控制。 | `ApiResult` |
| `ValidationException` | 输出字段级验证错误，错误码和状态码由验证异常选项控制。 | `ApiResultWithErrors` |
| `DbUpdateConcurrencyException` | 按并发冲突处理。 | `ApiResult` |
| 未知异常 | 按内部服务器错误处理。 | `ApiResult` |

`ValidationException.StatusCode` 同时用于 `FluentValidation.ValidationException`，以及 Web API 模型绑定或 `ValidationProblemDetails` 产生的验证失败结果。

错误消息会按错误码解析本地化资源。`BusinessException` 使用自身的 `ErrorCode` 和 `ErrorArgs`；验证失败、并发冲突和未知异常使用配置项中的框架级错误码。未启用本地化或资源不存在时，错误消息回退为错误码。

`BusinessException` 不保存本地化资源类型。项目需要自行决定业务异常资源文件的分组方式；常见做法包括按错误码前缀、聚合、模块或异常类型分组。需要自定义分组时，可以实现 `IBusinessExceptionResourceTypeResolver`。

```csharp
public class AppBusinessExceptionResourceTypeResolver
    : IBusinessExceptionResourceTypeResolver
{
    public Type Resolve(BusinessException exception)
        => exception.ErrorCode switch
        {
            string code when code.StartsWith("product_") => typeof(ProductResource),
            string code when code.StartsWith("order_") => typeof(OrderResource),
            string code when code.StartsWith("payment_") => typeof(PaymentResource),
            _ => typeof(SharedResource)
        };
}
```

在 `AddApiResultWrapping()` 中指定解析器：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddApiResultWrapping(options =>
    {
        options.BusinessException.UseResourceTypeResolver<AppBusinessExceptionResourceTypeResolver>();
    });
});
```

解析器返回的类型会用于创建 `IStringLocalizer`。例如 `typeof(OrderResource)` 对应订单聚合的资源文件；未匹配到专用资源时，通常返回 `SharedResource`。资源文件的组织方式见 [本地化：使用 IStringLocalizer](./localization.md#using-localizer)。

异常会同时写入日志。`BusinessException` 使用自身的 `LogLevel`；其它异常按未处理异常记录。日志作用域包含异常类型、错误码、HTTP 状态码和可选 `TraceId`；业务异常的 `Metadata` 会合并到日志作用域，不写入 API 响应。