# Application Flowing

The application layer organizes the execution flow of business use cases. It receives commands or queries, performs request-level validation, loads domain objects, coordinates external dependencies, and returns results to the presentation layer. Core business rules and consistency constraints should be maintained by the domain model.

This document describes how to organize application flow in Dddify projects and clarifies the responsibility boundaries of commands, queries, validators, handlers, repositories, unit of work, and domain events.

## Responsibility Boundaries

The application layer is suitable for the following responsibilities:

- Receiving `ICommand`, `ICommand<TResult>`, or `IQuery<TResult>` requests.
- Running request-level validation.
- Loading aggregates through repositories or reading query-oriented DTOs.
- Checking business conditions that require repositories, external services, or other resources.
- Calling business methods on aggregate roots, entities, or value objects.
- Coordinating application services, domain services, and infrastructure abstractions.
- Returning DTOs, identifiers, or operation results.

The application layer should not do the following:

- Modify aggregate internal state directly.
- Hold invariants that should be maintained by aggregates.
- Reference presentation-layer types such as Razor Pages, controllers, or HTTP context.
- Leak EF Core query details into the domain layer.
- Implement large amounts of state transition and domain calculation logic inside handlers.

## Commands

Commands represent use cases that may change system state. Commands without a return value implement `ICommand`; commands with a return value implement `ICommand<TResult>`. Command handlers implement `ICommandHandler<TCommand>` or `ICommandHandler<TCommand, TResult>`.

```csharp
public record PayOrderCommand(Guid OrderId) : ICommand;
```

Handlers should focus on orchestration: loading or creating aggregates, performing required checks, calling domain behavior, and returning results. Changes to aggregate internal state should be completed by aggregate methods.

### Modify an Existing Aggregate

When modifying an existing aggregate, the handler loads the aggregate through a repository and calls an aggregate method to express the business action.

```csharp
public class PayOrderCommandHandler(IOrderRepository orderRepository, IClock clock)
    : ICommandHandler<PayOrderCommand>
{
    public async Task Handle(PayOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(command.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(command.OrderId);

        order.Pay(clock.UtcNow);
    }
}
```

The handler should not set `order.Status` directly. Payment status validation and transition are maintained by `Order.Pay(...)`.

### Create a New Aggregate

When creating a new aggregate, the command handler prepares the external inputs required by the use case, calls the aggregate constructor or factory method, and adds the aggregate through a repository. The initial state and invariants of the aggregate should be maintained by the aggregate itself.

```csharp
public record PlaceOrderCommand(
    Guid BuyerId,
    Address ShippingAddress,
    IReadOnlyCollection<PlaceOrderItemDto> Items) : ICommand<Guid>;

public class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IGuidGenerator guidGenerator)
    : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Guid> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new Order(guidGenerator.Create(), command.BuyerId, command.ShippingAddress);

        foreach (var item in command.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
        }

        await orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
```

The handler should not bypass the aggregate constructor or business methods to fill internal state directly during creation.

### Check External Conditions

When checks require external data such as inventory, membership, coupons, or uniqueness, the handler can coordinate repositories, domain services, or application services. Rules that depend only on the internal state of a single aggregate should remain inside that aggregate.

```csharp
public class PlaceOrderCommandHandler(
    IOrderRepository orderRepository,
    IProductAvailabilityChecker availabilityChecker,
    IGuidGenerator guidGenerator)
    : ICommandHandler<PlaceOrderCommand, Guid>
{
    public async Task<Guid> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        foreach (var item in command.Items)
        {
            var available = await availabilityChecker.IsAvailableAsync(
                item.ProductId,
                item.Quantity,
                cancellationToken);

            if (!available)
            {
                throw new ProductUnavailableException(item.ProductId);
            }
        }

        var order = new Order(guidGenerator.Create(), command.BuyerId, command.ShippingAddress);

        foreach (var item in command.Items)
        {
            order.AddItem(item.ProductId, item.ProductName, item.UnitPrice, item.Quantity);
        }

        await orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
```

The application layer can coordinate multiple dependencies, but it should not move aggregate-internal rules into handlers.

## Queries

Queries represent read-only use cases and should not modify domain state. Query requests implement `IQuery<TResult>`.

```csharp
public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>;
```

Query handlers implement `IQueryHandler<TQuery, TResult>`. A handler can read an aggregate through a repository and map it to a DTO, or use a dedicated read model or query service.

```csharp
public class GetOrderByIdQueryHandler(IOrderReadRepository orderReadRepository)
    : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        return await orderReadRepository.GetDtoAsync(query.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(query.OrderId);
    }
}
```

Complex lists, paged statistics, and cross-aggregate views are usually better returned as DTOs or read models rather than aggregate roots.

## Validation

Dddify enables `ValidationBehavior<,>` by default to run FluentValidation validators before the handler executes. When validation fails, the pipeline throws `ValidationException`.

After `AddApiResultWrapping()` is enabled, Dddify converts `ValidationException` into a unified API response. If it is not enabled, the project is responsible for exception handling.

### Validators

Validators handle request-level rules and should inherit from `AbstractValidator<TRequest>`:

```csharp
public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(c => c.BuyerId).NotEmpty();
        RuleFor(c => c.ShippingAddress).NotNull();
        RuleFor(c => c.Items).NotEmpty();
    }
}
```

### Rule Boundaries

Rules suitable for validators:

- Required fields.
- String length.
- Numeric ranges.
- Enum values.
- Non-empty collections.
- Request format constraints.

Rules not suitable for validators:

- Aggregate state transitions.
- Monetary calculation consistency.
- Orders cannot be modified after payment.
- Whether an order can be cancelled.

These are domain rules and should be placed in aggregates, value objects, or domain services.

## Application Exceptions

Application exceptions express expected failures discovered by the application layer while orchestrating use cases. These failures usually come from use-case boundaries, access control, or external collaboration results rather than aggregate-internal invariants. Projects can define specific exceptions that inherit from `AppException`; `AppException` inherits from `BusinessException` and participates in unified business exception handling.

Common scenarios include missing data, insufficient permissions, duplicate submissions, occupied resources, and business failures returned by external services.

```csharp
public class OrderNotFoundException : AppException
{
    public OrderNotFoundException(Guid orderId)
    {
        WithErrorCode("order_not_found");
        WithMetadata("OrderId", orderId);
    }
}
```

Handlers should throw application exceptions with clear semantics instead of using `null`, `false`, or generic exceptions to express business failures:

```csharp
public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(query.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(query.OrderId);

        return new OrderDto(order.Id, order.BuyerId, order.Status);
    }
}
```

The error contract of application exceptions should remain stable for API callers:

- `WithErrorCode(...)`: sets a stable error code, also used as the localization resource key, and supports formatting arguments.
- `WithLogLevel(...)`: sets the log level for the exception.
- `WithMetadata(...)`: adds structured diagnostic information, such as business object identifiers or external dependency return codes.

Error code and metadata conventions:

- Use stable and semantically clear error codes.
- Error codes can be used for API responses, log retrieval, and localization resource matching.
- `Metadata` should not contain sensitive information such as passwords, tokens, or government identity numbers.

After `AddApiResultWrapping()` is enabled, Dddify automatically wraps application exceptions into unified API responses. If it is not enabled, exception handling, HTTP status codes, and response formats are the project's responsibility.

## DTOs and Mapping

DTOs define the data shape returned by the application layer and isolate domain objects from presentation-layer models. Commands and queries express use-case input; DTOs express use-case output. Mapping code converts data between domain objects, read models, and DTOs.

```csharp
public record OrderDto(
    Guid Id,
    Guid BuyerId,
    string Status,
    decimal TotalAmount,
    IReadOnlyCollection<OrderLineDto> Lines);

public record OrderLineDto(
    Guid ProductId,
    string ProductName,
    int Quantity,
    decimal Amount);
```

Projects can choose automatic or manual mapping according to use-case complexity. Both approaches can coexist in the same project.

Mapping recommendations:

- Do not expose EF Core entities directly to the presentation layer.
- Do not let DTOs enter the domain model.
- Prefer DTOs or read models for query output.
- Command input can use simple values, value objects, or dedicated input item types.
- Return only fields required by the current use case.
- Explicitly handle conversion of enums, value objects, and time formats.
- Do not return sensitive fields, internal state fields, or fields used only for persistence.
- Complex projections can be extracted to mapping configuration, private methods, query services, or read models.
- Materialize results when returning collections, and avoid exposing deferred execution sequences to callers.

### Automatic Mapping

Dddify integrates Mapster in `AddDddify()`. It scans mapping configuration and registers `TypeAdapterConfig` and `IMapper`. Automatic mapping is suitable when field structures are stable and conversion rules are simple. It can reduce repetitive assignment code.

Application code should inject `IMapper` for mapping. Direct use of Mapster's `Adapt` extension method is not recommended. Accessing mapping capabilities through `IMapper` keeps dependencies explicit, makes replacement easier in tests, and avoids scattering static extension methods across business code.

```csharp
public class GetOrderByIdQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(query.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(query.OrderId);

        return mapper.Map<OrderDto>(order);
    }
}
```

When mapping involves derived fields, value object expansion, or collection item conversion, use `IRegister` to centralize mapping rules.

```csharp
public class OrderMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.TotalAmount, src => src.Lines.Sum(line => line.Subtotal.Amount))
            .Map(dest => dest.Lines, src => src.Lines);

        config.NewConfig<OrderLine, OrderLineDto>()
            .Map(dest => dest.Amount, src => src.Subtotal.Amount);
    }
}
```

After configuration, the handler still only calls `mapper.Map<OrderDto>(order)`; total calculation, status conversion, and order line amount expansion are maintained by the mapping configuration.

### Manual Mapping

Manual mapping makes mapping logic explicit, field sources clear, and conversion rules easy to review. When DTO or domain object structures change, the compiler is more likely to expose mismatches.

```csharp
public class GetOrderByIdQueryHandler(IOrderRepository orderRepository)
    : IQueryHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetAsync(query.OrderId, cancellationToken)
            ?? throw new OrderNotFoundException(query.OrderId);

        return new OrderDto(
            order.Id,
            order.BuyerId,
            order.Status.ToString(),
            order.Lines.Sum(line => line.Subtotal.Amount),
            order.Lines.Select(line => new OrderLineDto(
                line.ProductId,
                line.ProductName,
                line.Quantity,
                line.Subtotal.Amount)).ToArray());
    }
}
```

## Unit of Work Pipeline {#work-unit-pipeline}

After `AddDbContextWithUnitOfWork<TContext>(...)` is enabled, the default `UnitOfWorkBehavior<,>` processes command requests in the MediatR pipeline.

`UnitOfWorkBehavior<,>` applies only to `ICommand` and `ICommand<TResult>`. It does not process `IQuery<TResult>` because queries should remain read-only. If a query requires consistent reads, the project should handle it separately according to database and business requirements.

When a command is executed, the unit-of-work behavior follows these rules:

- If there is no current transaction, it starts a new transaction.
- If there is already a current transaction, it joins the existing transaction.
- After the handler succeeds, it calls `IUnitOfWork.SaveChangesAsync(...)`.
- If the transaction was started by the current behavior, it commits the transaction.
- If an exception occurs, it rolls back the transaction started by the current behavior and rethrows the original exception.

Add `[SkipUnitOfWorkBehavior]` to a command type when the command needs to skip the unit-of-work pipeline:

```csharp
[SkipUnitOfWorkBehavior]
public record RecalculateOrderReadModelCommand(Guid OrderId) : ICommand;
```

Common scenarios:

- The command only triggers in-memory calculation, cache refresh, or local file processing and does not modify database state.
- The command rebuilds read models, synchronizes indexes, or sends persisted pending tasks, where the persistence boundary is controlled by an external process.
- The command needs to manage transactions explicitly or use transaction boundaries different from the default `IUnitOfWork`.

Do not use this attribute to bypass saving and transaction handling for ordinary write use cases.

## Domain Event Handling

After `AddDbContextWithUnitOfWork<TContext>(...)` is enabled, Dddify registers `DispatchDomainEventsInterceptor` to dispatch domain events collected by aggregates during EF Core's `SavingChanges` stage. The interceptor scans `IAggregateRoot` instances tracked by the current `DbContext`, publishes pending `DomainEvents`, and clears the event collection.

Domain events are published in process through MediatR. Handlers implement `IDomainEventHandler<TDomainEvent>`:

```csharp
public class OrderPaidEventHandler(ISender sender)
    : IDomainEventHandler<OrderPaidDomainEvent>
{
    public async Task Handle(OrderPaidDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(new CreateShipmentCommand(@event.OrderId), cancellationToken);
    }
}
```

Event handlers complete before the current `SaveChanges` continues. If the current command is inside a transaction managed by `UnitOfWorkBehavior<,>`, local writes performed by event handlers participate in the same unit of work and transaction.

Handler responsibility recommendations:

- Use handlers to trigger subsequent in-process application use cases, such as creating shipments, recording notification tasks, or writing integration events.
- Do not use handlers to repair aggregate-internal state that must be immediately consistent; such rules should be completed in aggregate methods.
- Avoid directly modifying other aggregates through repositories inside handlers. Send the corresponding `Command` when a write use case must be executed.
- Keep handlers short and avoid blocking the current save flow.

External service calls such as SMS, email, and Webhook calls should not be executed directly in synchronous domain event handlers. Event handlers are usually still inside the current save flow and database transaction boundary. Direct external calls introduce the following problems:

- They extend database transaction duration.
- External service failures may roll back the current command.
- External services cannot guarantee atomic commits with the local database transaction.
- Already sent external messages cannot be automatically undone by transaction rollback.

The recommended approach is to persist pending records within the current transaction, such as notification tasks, outbox records, or integration event records. After the transaction commits, a background job reads those records and calls external services.

```csharp
public class OrderPaidEventHandler(ISender sender)
    : IDomainEventHandler<OrderPaidDomainEvent>
{
    public async Task Handle(OrderPaidDomainEvent @event, CancellationToken cancellationToken)
    {
        await sender.Send(
            new CreateNotificationCommand(
                @event.OrderId,
                NotificationType.Email,
                "The order has been paid."),
            cancellationToken);
    }
}
```

`CreateNotificationCommand` saves the pending notification record and does not send the email directly. A background job should perform actual delivery after the transaction commits and record sending status, retry count, and error information.

When implementing background delivery, follow these conventions:

- Use stable business keys or message keys for idempotency to avoid duplicate sends during retries.
- Use retry and backoff strategies for transient failures, and record failure status for permanent failures.
- Record processing states such as `Pending`, `Processing`, `Succeeded`, and `Failed`.
- Keep request payloads, target URLs, response status, and error information for Webhooks or integration events to support troubleshooting.
- Decouple sending logic from domain event handlers so it does not block the current database transaction.

Only consider calling infrastructure services directly in event handlers when the business explicitly requires the current command to roll back if the external call fails, and the call itself is controlled, fast, and idempotent.

## Common Pitfalls

- **Handlers contain domain rules**: state transitions, amount calculations, and invariant checks are scattered across handlers.
- **Queries modify state**: `IQuery<TResult>` handlers perform writes, breaking read/write semantics and transaction expectations.
- **Aggregate methods are bypassed**: the application layer sets aggregate properties or internal collections directly, preventing domain rules from being applied centrally.
- **Command boundaries are too large**: a single command modifies multiple aggregates without a consistency relationship, expanding transaction scope and failure impact.
- **DTOs enter the domain layer**: DTOs are used as domain objects, causing presentation-layer models to pollute the domain model.
- **Event handlers perform external delivery**: SMS, email, or Webhook calls are sent synchronously, extending transactions and causing non-rollbackable effects.

## Checklist

- Does the command express one clear write use case?
- Does the query handler remain read-only and avoid modifying persistent state?
- Does the handler only orchestrate: load, check, call domain behavior, and return results?
- Are aggregate internal state changes completed through aggregate methods?
- Are request format, required fields, and length rules placed in FluentValidation validators?
- Are cross-aggregate or external resource checks coordinated by the application layer or domain services?
- Are DTOs used only for application input and output, and kept out of the domain model?
- Do write operations enter the unit-of-work boundary through commands?
- Do domain event handlers avoid repairing aggregate-internal consistency?
- Are external side effects handled by post-transaction background processes?
