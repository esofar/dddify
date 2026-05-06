# Domain Modeling

The domain layer expresses core business concepts, business rules, and consistency constraints. Dddify provides foundational types such as aggregate roots, entities, value objects, domain events, auditing interfaces, and repository contracts to help projects encapsulate business behavior in the domain model while staying decoupled from persistence implementations.

This document describes how to organize domain models and common conventions in Dddify projects.

## Modeling Process

When modeling, identify business boundaries first, then choose the appropriate modeling elements:

1. Identify core business concepts and business actions.
2. Distinguish objects that require stable identity from value objects.
3. Define aggregates around transactional consistency and business invariants.
4. Encapsulate core business rules in aggregate methods or value objects.
5. Use domain events to express business facts that have already occurred.
6. Use repository contracts to isolate persistence implementations.

In an order scenario, `Order` acts as the aggregate root and maintains order status, order lines, payment status, and amount consistency.

## Aggregate Roots

An aggregate root is the only external entry point of an aggregate and is responsible for maintaining consistency within the aggregate boundary. Dddify provides `AggregateRoot<TKey>` as the base type for aggregate roots.

```csharp
public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderLine> _lines = [];

    private Order() { }

    public Order(Guid id, Guid buyerId, Address shippingAddress)
    {
        Id = id;
        BuyerId = buyerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;

        AddDomainEvent(new OrderPlacedDomainEvent(Id, BuyerId));
    }

    public Guid BuyerId { get; private set; }

    public Address ShippingAddress { get; private set; } = default!;

    public OrderStatus Status { get; private set; }

    public DateTime? PaidAt { get; private set; }

    public IReadOnlyCollection<OrderLine> Lines => _lines.AsReadOnly();
}
```

Modeling conventions:

- Aggregate roots expose executable actions through business methods and avoid direct external updates to key properties.
- Constructors or factory methods should ensure that a new aggregate is in a valid state.
- A private parameterless constructor can be kept for EF Core.
- Aggregates maintain only their own consistency and do not directly modify the state of other aggregates.
- Internal aggregate collections should be exposed through read-only views; modification operations should be completed by aggregate methods.

### Modify State Through Behavior

Aggregate methods should express business intent and perform state changes and rule checks internally.

```csharp
public void AddItem(Guid productId, string productName, Money unitPrice, int quantity)
{
    EnsurePending();

    if (quantity <= 0)
    {
        throw new InvalidOrderQuantityException(quantity);
    }

    _lines.Add(new OrderLine(Guid.NewGuid(), Id, productId, productName, unitPrice, quantity));
}

public void Pay(DateTime paidAt)
{
    EnsurePending();

    if (_lines.Count == 0)
    {
        throw new EmptyOrderCannotBePaidException(Id);
    }

    Status = OrderStatus.Paid;
    PaidAt = paidAt;

    AddDomainEvent(new OrderPaidDomainEvent(Id));
}
```

Callers initiate business actions such as "add item" or "pay order" and do not operate on aggregate internal fields directly.

### Encapsulate Invariants

Invariants are business constraints that an aggregate must satisfy at all times. Rules that depend only on aggregate internal state should be placed inside the aggregate.

```csharp
private void EnsurePending()
{
    if (Status != OrderStatus.Pending)
    {
        throw new OrderStatusChangedException(Id, Status);
    }
}
```

Rule placement conventions:

- Consistency rules that depend only on a single aggregate's state belong inside the aggregate.
- Rules that require repository queries or external service calls belong in application handlers or domain services.
- Request-level rules such as required fields, length, and format belong in FluentValidation validators.

## Entities

Entities have stable identity. Aggregates can contain internal entities, but internal entities should not be modified independently outside the aggregate root.

```csharp
public class OrderLine : Entity<Guid>
{
    private OrderLine() { }

    public OrderLine(Guid id, Guid orderId, Guid productId, string productName, Money unitPrice, int quantity)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public string ProductName { get; private set; } = string.Empty;

    public Money UnitPrice { get; private set; } = default!;

    public int Quantity { get; private set; }

    public Money Subtotal => UnitPrice.Multiply(Quantity);
}
```

Order lines are created and maintained by the aggregate root `Order`. External code should modify aggregate state through `Order.AddItem(...)`, not by creating or modifying order lines directly.

## Auditing

Dddify provides auditing-related base classes and interfaces to declare auditing capabilities on domain objects. EF Core integration fills auditing fields before saving.

When an aggregate root needs to record creation, modification, and deletion information, inherit from `AuditableAggregateRoot<TKey>`:

```csharp
public class Order : AuditableAggregateRoot<Guid>
{
    private Order() { }

    public Order(Guid id, Guid buyerId, Address shippingAddress)
    {
        Id = id;
        BuyerId = buyerId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
    }

    public Guid BuyerId { get; private set; }

    public Address ShippingAddress { get; private set; } = default!;

    public OrderStatus Status { get; private set; }
}
```

`AuditableAggregateRoot<TKey>` contains the following properties:

- `CreatedBy`, `CreatedAt`
- `ModifiedBy`, `ModifiedAt`
- `IsDeleted`
- `DeletedBy`, `DeletedAt`

When an internal aggregate entity needs auditing capabilities, inherit from `AuditableEntity<TKey>`:

```csharp
public class OrderLine : AuditableEntity<Guid>
{
    private OrderLine() { }

    internal OrderLine(Guid id, Guid orderId, Guid productId, Money unitPrice, int quantity)
    {
        Id = id;
        OrderId = orderId;
        ProductId = productId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public Guid OrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public Money UnitPrice { get; private set; } = default!;

    public int Quantity { get; private set; }
}
```

If the built-in base classes do not fit the project's entity base class design, implement more fine-grained auditing interfaces as needed:

- `IHasCreatedBy`, `IHasCreatedAt`
- `IHasModifiedBy`, `IHasModifiedAt`
- `IHasDeletedBy`, `IHasDeletedAt`
- `ICreationAuditable`, `IModificationAuditable`, `IDeletionAuditable`, `IAuditable`

After `AddDbContextWithUnitOfWork<TContext>()` is enabled, Dddify fills auditing fields before saving data. The user identifier comes from `ICurrentUser`, which requires `AddCurrentUser(...)`; the time comes from `IClock`, which requires `AddTiming(...)`. The auditing time source can be configured through `AuditTimeSource`.

Deletion auditing applies only to soft deletion. The entity must implement `ISoftDeletable` and have `IHasDeletedBy` and `IHasDeletedAt` properties before Dddify writes deletion auditing information during soft deletion. Physical deletion does not retain deletion auditing fields.

## Soft Delete

Entities that implement `ISoftDeletable` are treated as supporting soft delete.

```csharp
public class Order : AggregateRoot<Guid>, ISoftDeletable
{
    private Order() { }

    public bool IsDeleted { get; set; }
}
```

`AuditableAggregateRoot<TKey>` and `AuditableEntity<TKey>` already include `IsDeleted`.

After `ApplyDefaultConventions()` is called in `DbContext`, Dddify configures a global query filter for soft-delete entities and returns only data where `IsDeleted == false`.

When a repository or `DbContext` deletes an entity that supports soft delete, the operation is converted to an update before saving, and `IsDeleted` is set to `true`. If the entity also has deletion auditing properties, `DeletedBy` and `DeletedAt` are written as well.

## Concurrency Stamp

Implement `IHasConcurrencyStamp` when optimistic concurrency control is required.

```csharp
public class Order : AuditableAggregateRoot<Guid>, IHasConcurrencyStamp
{
    private Order() { }

    public string? ConcurrencyStamp { get; set; }
}
```

After `ApplyDefaultConventions()` is called in `DbContext`, Dddify configures `ConcurrencyStamp` as an EF Core concurrency token. When an entity is added or modified, the concurrency stamp is refreshed before saving.

When update or delete APIs need to detect the client data version, require the client to submit the `ConcurrencyStamp` from the previous read. The server sets the original concurrency stamp through the repository, and EF Core performs the concurrency check during saving.

```csharp
var order = await orderRepository.GetAsync(command.OrderId, cancellationToken)
    ?? throw new OrderNotFoundException(command.OrderId);

orderRepository.SetOriginalConcurrencyStamp(order, command.ConcurrencyStamp);

order.Pay(clock.UtcNow);
```

If the concurrency stamp in the database differs from the value submitted by the client, EF Core throws `DbUpdateConcurrencyException`. After `AddApiResultWrapping()` is enabled, Dddify converts the exception into a unified API response. If it is not enabled, the project is responsible for exception handling.

## Value Objects

Value objects do not have independent identity and are usually compared by property values. Dddify provides `ValueObject`; use `GetEqualityComponents()` to declare the components that participate in equality comparison.

```csharp
public sealed class Money : ValueObject
{
    public Money(decimal amount, string currency)
    {
        if (amount < 0)
        {
            throw new NegativeMoneyException(amount);
        }

        Amount = amount;
        Currency = currency;
    }

    public decimal Amount { get; }

    public string Currency { get; }

    public Money Multiply(int quantity)
        => new(Amount * quantity, Currency);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
```

Suitable scenarios for value objects:

- Money, address, time range, and specification parameters.
- Multiple fields always appear together and must be validated together.
- Object equality is determined by field values rather than `Id`.

## Enums

For fixed values that do not require additional behavior, prefer C# `enum`.

```csharp
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Shipped = 3,
    Cancelled = 4
}
```

When enum items need display names, behavior, additional properties, or parsing capabilities, use Dddify's `Enumeration` base class.

```csharp
public sealed class ShippingMethod : Enumeration
{
    public static readonly ShippingMethod Standard = new(1, "Standard", TimeSpan.FromDays(5));
    public static readonly ShippingMethod Express = new(2, "Express", TimeSpan.FromDays(2));
    public static readonly ShippingMethod SameDay = new(3, "SameDay", TimeSpan.FromHours(12));

    private ShippingMethod(int id, string name, TimeSpan estimatedDelivery)
        : base(id, name)
    {
        EstimatedDelivery = estimatedDelivery;
    }

    public TimeSpan EstimatedDelivery { get; }

    public bool IsFast => EstimatedDelivery <= TimeSpan.FromDays(2);
}
```

Enumeration classes can be parsed by value or name through built-in methods:

```csharp
var method = Enumeration.FromValue<ShippingMethod>(2);
var sameDay = Enumeration.FromDisplayName<ShippingMethod>("SameDay");
```

When persisting enumeration classes with EF Core, use `HasEnumerationConversion()` to save enumeration items as numeric values:

```csharp
builder.Property(x => x.ShippingMethod)
    .HasEnumerationConversion();
```

Selection conventions:

- Prefer C# `enum` for simple state transitions.
- Use `Enumeration` when items need behavior, display names, properties, or parsing capabilities.
- Do not convert all `enum` types into enumeration classes only for abstraction consistency.

## Domain Events

Domain events express facts that have already occurred in the domain. Dddify domain events implement `IDomainEvent`.

```csharp
public record OrderPlacedDomainEvent(Guid OrderId, Guid BuyerId) : IDomainEvent;

public record OrderPaidDomainEvent(Guid OrderId) : IDomainEvent;
```

After an aggregate root inherits from `AggregateRoot<TKey>`, it can collect domain events through `AddDomainEvent(...)`:

```csharp
AddDomainEvent(new OrderPaidDomainEvent(Id));
```

Usage conventions:

- Name events in the past tense, for example `OrderPaidDomainEvent`.
- Carry only the minimum data required by handlers.
- Do not use domain events to replace rules that must be completed immediately inside the aggregate.
- State that must remain immediately consistent within the same aggregate should be updated in aggregate methods.

After `AddDbContextWithUnitOfWork<TContext>()` is enabled, Dddify dispatches domain events collected by aggregates through an interceptor during `SaveChanges`.

## Repository Contracts

Repositories provide collection-like access to aggregate roots and isolate persistence details such as EF Core. The domain layer defines repository contracts, and the infrastructure layer implements them.

Dddify provides `IRepository<TEntity, TKey>` as the base repository contract, including methods for reading, adding, updating, deleting, and setting concurrency stamps. Project repositories can extend it with methods that have clear business meaning.

```csharp
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
```

The infrastructure layer can inherit from `RepositoryBase<TDbContext, TEntity, TKey>` to implement repositories:

```csharp
public sealed class OrderRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, Order, Guid>(context), IOrderRepository
{
    public Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        => context.Orders.AnyAsync(x => x.OrderNumber == orderNumber, cancellationToken);
}
```

Usage principles:

- Repositories are organized around aggregate roots; do not create separate repositories for aggregate-internal entities.
- The domain layer defines only contracts and does not reference EF Core directly.
- The infrastructure layer implements repositories and handles persistence-framework details.
- Command handlers load and save aggregates through repositories, but repositories do not commit transactions.
- When a query has business meaning, prefer defining an explicit repository method.
- Use `AsQueryable()` only for a small number of queries that are difficult to express with explicit methods; complex read models should be organized separately in the application layer or query services.

## Domain Exceptions

Domain exceptions express that the domain model rejects a state or behavior that violates business rules. Aggregates, entities, value objects, or domain services can define specific exceptions that inherit from `DomainException` when maintaining invariants. `DomainException` inherits from `BusinessException` and participates in unified business exception handling.

Common scenarios include disallowed state transitions, invalid amount or quantity constraints, insufficient inventory, and invalid value object parameters.

```csharp
public class EmptyOrderCannotBePaidException : DomainException
{
    public EmptyOrderCannotBePaidException(Guid orderId)
    {
        WithErrorCode("empty_order_cannot_be_paid");
        WithMetadata("OrderId", orderId);
    }
}
```

Domain objects can throw domain exceptions directly while maintaining invariants:

```csharp
public void Pay(DateTime paidAt)
{
    if (_lines.Count == 0)
    {
        throw new EmptyOrderCannotBePaidException(Id);
    }

    Status = OrderStatus.Paid;
    PaidAt = paidAt;
}
```

The error contract of domain exceptions should describe rule failures in domain language:

- `WithErrorCode(...)`: sets a stable error code, also used as the localization resource key, and supports formatting arguments.
- `WithLogLevel(...)`: sets the log level for the exception.
- `WithMetadata(...)`: adds structured diagnostic information, such as business object identifiers, current state, or rule parameters.

Error code and metadata conventions:

- Use stable and semantically clear error codes.
- Error codes can be used for API responses, log retrieval, and localization resource matching.
- `Metadata` should not contain sensitive information such as passwords, tokens, or government identity numbers.
- Domain exceptions should not contain application-layer or presentation-layer information such as HTTP status codes, current user, or request context.
- Technical errors such as database or network failures should not be wrapped as domain exceptions.

After `AddApiResultWrapping()` is enabled, Dddify automatically wraps domain exceptions into unified API responses. If it is not enabled, exception handling, HTTP status codes, and response formats are the project's responsibility.

## Common Pitfalls

- **Anemic model**: aggregates contain only properties and simple assignments, while business rules are scattered across handlers or services.
- **Oversized aggregate**: order, inventory, payment, logistics, and other consistency boundaries are placed in the same aggregate.
- **Bypassing aggregate roots to modify entities**: external code directly operates on aggregate-internal entities and breaks invariants.
- **Domain event abuse**: event handlers are used to fix state that must be immediately consistent.
- **Repository abuse**: a repository is created for every entity, or the application layer builds many query expressions manually.
- **Premature abstraction**: many domain services, generic base classes, and unified interfaces are created before business rules are stable.
- **Infrastructure dependency**: domain objects reference EF Core, current user, time, or external service implementations.

## Checklist

- Are aggregate boundaries defined around transactional consistency and business invariants?
- Do aggregate roots express business actions through methods?
- Can constructors and factory methods ensure that the initial aggregate state is valid?
- Are key properties and internal collections protected from direct external modification?
- Can aggregate-internal entities be maintained only through the aggregate root?
- Do value objects encapsulate their own validation, equality, and required behavior?
- Are enumeration classes used only when enum scenarios really require additional properties or behavior?
- Do domain events express facts that have already occurred and carry only required data?
- Are rules that must be immediately consistent completed in aggregate methods?
- Are repository contracts defined only for aggregate roots and placed in the domain layer?
- Do repository methods express clear business intent?
- Are domain exceptions used for expected business errors and do they provide stable error codes?
- Does the domain layer avoid referencing infrastructure and presentation-layer types?
- Do application handlers only orchestrate use cases instead of containing core domain rules?
