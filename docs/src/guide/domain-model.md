# 领域建模

领域层用于表达核心业务概念、业务规则和一致性约束。Dddify 提供聚合根、实体、值对象、领域事件、审计接口和仓储契约等基础类型，帮助项目在领域模型中封装业务行为，并与持久化实现保持解耦。

本文说明 Dddify 项目中领域模型的组织方式和常用约定。

## 建模流程

建模时应先识别业务边界，再选择合适的建模元素：

1. 识别核心业务概念和业务动作。
2. 区分需要稳定身份的对象和值对象。
3. 按事务一致性和业务不变量划分聚合。
4. 将核心业务规则封装到聚合方法或值对象中。
5. 使用领域事件表达已经发生的业务事实。
6. 通过仓储契约隔离持久化实现。

以订单场景为例，`Order` 作为聚合根，负责维护订单状态、订单行、支付状态和金额一致性。

## 聚合根

聚合根是聚合的唯一外部入口，负责维护聚合边界内的一致性。Dddify 提供 `AggregateRoot<TKey>` 作为聚合根基础类型。

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

建模约定：

- 聚合根通过业务方法暴露可执行动作，避免外部直接设置关键属性。
- 构造函数或工厂方法应保证新聚合处于有效状态。
- 私有无参构造函数可保留给 EF Core 使用。
- 聚合只维护自身一致性，不直接修改其它聚合状态。
- 聚合内部集合应通过只读视图暴露，修改操作由聚合方法完成。

### 通过行为修改状态

聚合方法应表达业务意图，并在方法内部完成状态变更和规则校验。

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

调用方只发起“添加商品”“支付订单”等业务动作，不直接操作聚合内部字段。

### 封装不变量

不变量是聚合在任何时刻都必须满足的业务约束。只依赖聚合内部状态的规则应放在聚合内部。

```csharp
private void EnsurePending()
{
    if (Status != OrderStatus.Pending)
    {
        throw new OrderStatusChangedException(Id, Status);
    }
}
```

规则放置约定：

- 只依赖单个聚合状态的一致性规则，放在聚合内部。
- 需要查询仓储或调用外部服务的规则，放在应用处理器或领域服务中。
- 必填、长度、格式等请求级规则，放在 FluentValidation 验证器中。

## 实体

实体具有稳定身份。聚合内部可以包含实体，但内部实体不应脱离聚合根独立修改。

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

订单行由聚合根 `Order` 创建和维护。外部代码应通过 `Order.AddItem(...)` 修改聚合状态，而不是直接创建或修改订单行。

## 审计

Dddify 提供审计相关基类和接口，用于声明领域对象具备的审计能力。审计字段的填充由 EF Core 集成在保存前完成。

聚合根需要记录创建、修改、删除信息时，可以继承 `AuditableAggregateRoot<TKey>`：

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

`AuditableAggregateRoot<TKey>` 包含以下属性：

- `CreatedBy`、`CreatedAt`
- `ModifiedBy`、`ModifiedAt`
- `IsDeleted`
- `DeletedBy`、`DeletedAt`

聚合内部实体需要审计能力时，可以继承 `AuditableEntity<TKey>`：

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

如果内置基类不符合项目实体基类设计，可以按需实现更细粒度的审计接口：

- `IHasCreatedBy`、`IHasCreatedAt`
- `IHasModifiedBy`、`IHasModifiedAt`
- `IHasDeletedBy`、`IHasDeletedAt`
- `ICreationAuditable`、`IModificationAuditable`、`IDeletionAuditable`、`IAuditable`

启用 `AddDbContextWithUnitOfWork<TContext>()` 后，Dddify 会在保存数据前填充审计字段。用户标识来自 `ICurrentUser`，需通过 `AddCurrentUser(...)` 注册；时间来自 `IClock`，需通过 `AddTiming(...)` 注册。审计时间来源可通过 `AuditTimeSource` 配置。

删除审计仅适用于软删除。实体需实现 `ISoftDeletable`，并具备 `IHasDeletedBy`、`IHasDeletedAt` 属性，Dddify 才会在软删除时写入删除审计信息。物理删除不会保留删除审计字段。

## 软删除

实现 `ISoftDeletable` 的实体会被视为支持软删除。

```csharp
public class Order : AggregateRoot<Guid>, ISoftDeletable
{
    private Order() { }

    public bool IsDeleted { get; set; }
}
```

`AuditableAggregateRoot<TKey>` 和 `AuditableEntity<TKey>` 已包含 `IsDeleted`。

在 `DbContext` 中调用 `ApplyDefaultConventions()` 后，Dddify 会为软删除实体配置全局查询过滤器，仅返回 `IsDeleted == false` 的数据。

当仓储或 `DbContext` 删除支持软删除的实体时，保存前会被转换为更新操作，并设置 `IsDeleted = true`。如果实体同时具备删除审计属性，还会写入 `DeletedBy` 和 `DeletedAt`。

## 并发戳

需要乐观并发控制时，可以实现 `IHasConcurrencyStamp`。

```csharp
public class Order : AuditableAggregateRoot<Guid>, IHasConcurrencyStamp
{
    private Order() { }

    public string? ConcurrencyStamp { get; set; }
}
```

在 `DbContext` 中调用 `ApplyDefaultConventions()` 后，Dddify 会将 `ConcurrencyStamp` 配置为 EF Core 并发标记。实体新增或修改时，保存前会刷新并发戳。

更新或删除接口需要检测客户端数据版本时，可要求客户端提交上一次读取到的 `ConcurrencyStamp`。服务端通过仓储设置原始并发戳，由 EF Core 在保存时执行并发检查。

```csharp
var order = await orderRepository.GetAsync(command.OrderId, cancellationToken)
    ?? throw new OrderNotFoundException(command.OrderId);

orderRepository.SetOriginalConcurrencyStamp(order, command.ConcurrencyStamp);

order.Pay(clock.UtcNow);
```

如果数据库中的并发戳与客户端提交值不一致，EF Core 会抛出 `DbUpdateConcurrencyException`。启用 `AddApiResultWrapping()` 后，Dddify 会将该异常转换为统一 API 响应；未启用时，异常处理由项目自行负责。

## 值对象

值对象没有独立身份，通常由属性值定义相等性。Dddify 提供 `ValueObject`，通过 `GetEqualityComponents()` 声明参与相等性比较的组件。

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

适合建模为值对象的场景：

- 金额、地址、时间范围、规格参数。
- 多个字段总是一起出现，并需要一起校验。
- 对象相等性由字段值决定，而不是由 `Id` 决定。

## 枚举

固定取值且不需要附加行为的状态，优先使用 C# `enum`。

```csharp
public enum OrderStatus
{
    Pending = 1,
    Paid = 2,
    Shipped = 3,
    Cancelled = 4
}
```

当枚举项需要显示名、行为、附加属性或解析能力时，可以使用 Dddify 提供的 `Enumeration` 基类。

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

枚举类可以通过内置方法按值或名称解析：

```csharp
var method = Enumeration.FromValue<ShippingMethod>(2);
var sameDay = Enumeration.FromDisplayName<ShippingMethod>("SameDay");
```

使用 EF Core 持久化枚举类时，可以通过 `HasEnumerationConversion()` 将枚举项保存为数字值：

```csharp
builder.Property(x => x.ShippingMethod)
    .HasEnumerationConversion();
```

选择约定：

- 简单状态流转优先使用 C# `enum`。
- 枚举项需要行为、显示名、属性或解析能力时，使用 `Enumeration`。
- 不因抽象一致性将所有 `enum` 改为枚举类。

## 领域事件

领域事件用于表达领域中已经发生的事实。Dddify 的领域事件实现 `IDomainEvent`。

```csharp
public record OrderPlacedDomainEvent(Guid OrderId, Guid BuyerId) : IDomainEvent;

public record OrderPaidDomainEvent(Guid OrderId) : IDomainEvent;
```

聚合根继承 `AggregateRoot<TKey>` 后，可以通过 `AddDomainEvent(...)` 收集领域事件：

```csharp
AddDomainEvent(new OrderPaidDomainEvent(Id));
```

使用约定：

- 事件命名使用过去式，例如 `OrderPaidDomainEvent`。
- 事件只携带处理方需要的最小数据。
- 不使用领域事件替代聚合内部必须立即完成的规则。
- 同一聚合内必须立即保持一致的状态，应在聚合方法中完成。

启用 `AddDbContextWithUnitOfWork<TContext>()` 后，Dddify 会通过拦截器在 `SaveChanges` 过程中分发聚合收集的领域事件。

## 仓储契约

仓储为聚合根提供集合式访问，并隔离 EF Core 等持久化细节。领域层定义仓储契约，基础设施层负责实现。

Dddify 提供 `IRepository<TEntity, TKey>` 作为基础仓储契约，包含读取、添加、更新、删除和并发戳设置等方法。项目仓储可以在此基础上扩展业务含义明确的方法。

```csharp
public interface IOrderRepository : IRepository<Order, Guid>
{
    Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
}
```

基础设施层可以继承 `RepositoryBase<TDbContext, TEntity, TKey>` 实现仓储：

```csharp
public sealed class OrderRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, Order, Guid>(context), IOrderRepository
{
    public Task<bool> ExistsByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
        => context.Orders.AnyAsync(x => x.OrderNumber == orderNumber, cancellationToken);
}
```

使用原则：

- 仓储以聚合根为单位，不为聚合内部实体单独创建仓储。
- 领域层只定义契约，不直接引用 EF Core。
- 基础设施层实现仓储，并处理持久化框架相关细节。
- 命令处理器通过仓储加载和保存聚合，但不在仓储中提交事务。
- 查询具有业务含义时，优先定义明确的仓储方法。
- `AsQueryable()` 仅用于难以通过明确方法表达的少量查询；复杂读模型建议在应用层或查询服务中单独组织。

## 领域异常

领域异常用于表达领域模型拒绝违反业务规则的状态或行为。聚合、实体、值对象或领域服务在维护不变量时，可以定义具体异常并继承 `DomainException`；`DomainException` 继承自 `BusinessException`，会参与统一业务异常处理。

常见场景包括状态不允许流转、金额或数量不满足约束、库存不足、值对象参数无效等。

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

领域对象在维护不变量时可以直接抛出领域异常：

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

领域异常的错误契约应使用领域语言描述规则失败：

- `WithErrorCode(...)`：设置稳定错误码，同时作为本地化资源 key，并支持格式化参数。
- `WithLogLevel(...)`：设置异常对应的日志级别。
- `WithMetadata(...)`：添加结构化诊断信息，例如业务对象标识、当前状态或规则参数。

错误码和元数据约定：

- 使用稳定、语义明确的错误码。
- 错误码可用于 API 响应、日志检索和本地化资源匹配。
- `Metadata` 不应包含密码、令牌、身份证号等敏感信息。
- 领域异常不应包含 HTTP 状态码、当前用户、请求上下文等应用层或表现层信息。
- 数据库、网络等技术错误不应包装为领域异常。

启用 `AddApiResultWrapping()` 后，Dddify 会自动将领域异常包装为统一 API 响应；未启用时，异常处理、HTTP 状态码和响应格式由项目自行负责。

## 常见误区

- **贫血模型**：聚合只有属性和简单赋值，业务规则散落在处理器或服务中。
- **聚合过大**：将订单、库存、支付、物流等不同一致性边界放入同一个聚合。
- **绕过聚合根修改实体**：外部代码直接操作聚合内部实体，破坏不变量。
- **领域事件滥用**：把必须立即一致的状态修正交给事件处理器。
- **仓储滥用**：为每个实体创建仓储，或在应用层大量拼接查询表达式。
- **过早抽象**：在业务规则尚不稳定时创建大量领域服务、通用基类和统一接口。
- **依赖基础设施**：在领域对象中引用 EF Core、当前用户、时间或外部服务实现。

## 检查清单

- 聚合边界是否围绕事务一致性和业务不变量划分？
- 聚合根是否通过方法表达业务动作？
- 构造函数和工厂方法是否能保证聚合初始状态有效？
- 关键属性和内部集合是否避免被外部直接修改？
- 聚合内部实体是否只能通过聚合根维护？
- 值对象是否封装自身校验、相等性和必要行为？
- 枚举类是否只用于确实需要附加属性或行为的枚举场景？
- 领域事件是否表达已经发生的事实，并只携带必要数据？
- 必须立即一致的规则是否已在聚合方法中完成？
- 仓储契约是否只面向聚合根，并放在领域层？
- 仓储方法是否表达明确业务意图？
- 领域异常是否用于可预期业务错误，并提供稳定错误码？
- 领域层是否避免引用基础设施和表现层类型？
- 应用处理器是否只负责编排用例，而不是承载核心领域规则？

