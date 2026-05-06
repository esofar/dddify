# Result Wrapping

Dddify provides a unified API response structure for ASP.NET Core Web API. It defines the output format for successful responses, failed responses, field-level validation errors, and trace identifiers.

## Registration

Enable the API result wrapping module in `AddDddify`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddApiResultWrapping();
});
```

`AddApiResultWrapping()` enables the following for Web API actions with `[ApiController]`:

- Wrapping known JSON/API return values.
- Mapping unhandled exceptions to unified failure responses.

## Configuration

`AddApiResultWrapping(...)` can configure trace identifiers in responses, framework-level error codes, exception HTTP status codes, and the default status code and resource resolver for business exceptions.

`ApiResultWrappingOptions` options:

| Option | Default | Description |
| --- | --- | --- |
| `EnableTraceIdentifier` | `true` | Whether to output `traceId` in responses. |
| `ValidationException.ErrorCode` | `validation_failed` | Error code used for validation failures. |
| `ValidationException.StatusCode` | `400` | HTTP status code used for validation failures. |
| `ConcurrencyException.ErrorCode` | `concurrency_conflict` | Error code used for concurrency conflicts. |
| `ConcurrencyException.StatusCode` | `409` | HTTP status code used for concurrency conflicts. |
| `InternalServerException.ErrorCode` | `internal_server_error` | Error code used for unknown exceptions. |
| `InternalServerException.StatusCode` | `500` | HTTP status code used for unknown exceptions. |
| `BusinessException.StatusCode` | `200` | Default HTTP status code used for business exceptions. |
| `BusinessException.UseResourceTypeResolver` | `SharedResource` | Default resource type used for business exception error messages. |

## Response Structure

API result wrapping uses `ApiResult`, `ApiResult<T>`, and `ApiResultWithErrors` to represent unified response structures. Serialized JSON uses the following fields:

- `success`: indicates whether the request succeeded.
- `data`: data for a successful response; output only when a return value exists.
- `errorCode`: stable error code for a failed response.
- `errorMessage`: error message for a failed response, usually resolved from localization resources or falling back to the error code.
- `errors`: field-level validation errors, represented as a mapping from field names to arrays of error messages.
- `traceId`: request trace identifier.

Successful response example:

```json
{
  "success": true,
  "data": {
    "id": "8a0f4f1e-1f8a-4b5d-9b9f-4ad7a5a2d111"
  },
  "traceId": "0HMS..."
}
```

Failed response example:

```json
{
  "success": false,
  "errorCode": "order_not_found",
  "errorMessage": "Order does not exist.",
  "traceId": "0HMS..."
}
```

Validation failure response example:

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

## Result Wrapping

Result wrapping applies only to actions with `[ApiController]` and only handles known JSON/API return values. Unrecognized `IActionResult` results remain unchanged, such as `FileResult`, `RedirectResult`, and streaming responses.

Successful results:

- `ObjectResult` and `OkObjectResult` are wrapped as `ApiResult<T>`.
- `NoContentResult`, `EmptyResult`, and non-error `StatusCodeResult` are wrapped as `ApiResult`.

Failed results:

- `ValidationProblemDetails`, `SerializableError`, and `ModelStateDictionary` are wrapped as `ApiResultWithErrors`.
- `ProblemDetails` is wrapped as `ApiResult`, and `Extensions["code"]` is used as the error code first.
- 4xx and 5xx `StatusCodeResult` are wrapped as failed `ApiResult`.

If an endpoint must return a predefined raw JSON structure, use `[DisableResultWrapping]` to disable result wrapping. This scenario is common for third-party callback confirmation responses:

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

## Exception Mapping {#exception-mapping}

After API result wrapping is enabled, unhandled exceptions in actions are converted to failed responses. Default mapping rules:

| Exception Type | Handling | Response |
| --- | --- | --- |
| `BusinessException` | Uses the exception's own `ErrorCode`; status code is controlled by business exception options. | `ApiResult` |
| `ValidationException` | Outputs field-level validation errors; error code and status code are controlled by validation exception options. | `ApiResultWithErrors` |
| `DbUpdateConcurrencyException` | Handled as a concurrency conflict. | `ApiResult` |
| Unknown exception | Handled as an internal server error. | `ApiResult` |

`ValidationException.StatusCode` applies to both `FluentValidation.ValidationException` and validation failure results produced by Web API model binding or `ValidationProblemDetails`.

Error messages are resolved from localization resources by error code. `BusinessException` uses its own `ErrorCode` and `ErrorArgs`; validation failures, concurrency conflicts, and unknown exceptions use framework-level error codes from configuration. If localization is not enabled or the resource does not exist, the error message falls back to the error code.

`BusinessException` does not store a localization resource type. Projects should decide how to group business exception resource files. Common approaches include grouping by error code prefix, aggregate, module, or exception type. To customize grouping, implement `IBusinessExceptionResourceTypeResolver`.

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

Specify the resolver in `AddApiResultWrapping()`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddApiResultWrapping(options =>
    {
        options.BusinessException.UseResourceTypeResolver<AppBusinessExceptionResourceTypeResolver>();
    });
});
```

The type returned by the resolver is used to create `IStringLocalizer`. For example, `typeof(OrderResource)` corresponds to the resource file for the order aggregate. When no dedicated resource is matched, `SharedResource` is usually returned. See [Localization: Using IStringLocalizer](./localization.md#using-localizer) for resource file organization.

Exceptions are also written to logs. `BusinessException` uses its own `LogLevel`; other exceptions are logged as unhandled exceptions. The logging scope includes exception type, error code, HTTP status code, and optional `TraceId`; `Metadata` from business exceptions is merged into the logging scope and is not written to the API response.
