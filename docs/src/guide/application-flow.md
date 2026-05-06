# 应用编排

应用层用于组织业务用例的执行流程。它接收命令或查询，完成请求级验证、加载领域对象、协调外部依赖，并向表现层返回结果。核心业务规则和一致性约束应由领域模型维护。

本文说明 Dddify 项目中应用编排的组织方式，以及命令、查询、验证器、处理器、仓储、工作单元和领域事件的职责边界。

## 职责边界

应用层适合处理以下内容：

- 接收 `ICommand`、`ICommand<TResult>` 或 `IQuery<TResult>` 请求。
- 执行请求级验证。
- 通过仓储加载聚合，或读取面向查询的 DTO。
- 检查需要仓储、外部服务或其它资源参与的业务条件。
- 调用聚合根、实体或值对象的业务方法。
- 协调应用服务、领域服务和基础设施抽象。
- 返回 DTO、标识符或操作结果。

应用层不应处理以下内容：

- 直接修改聚合内部状态。
- 承载本应由聚合维护的不变量。
- 引用 Razor Pages、Controller、HTTP 上下文等表现层类型。
- 将 EF Core 查询细节扩散到领域层。
- 在处理器中实现大量状态流转和领域计算。

## 命令

命令表示可能改变系统状态的用例。无返回值命令实现 `ICommand`，有返回值命令实现 `ICommand<TResult>`。命令处理器实现 `ICommandHandler<TCommand>` 或 `ICommandHandler<TCommand, TResult>`。

```csharp
public record PayOrderCommand(Guid OrderId) : ICommand;
```

处理器应以编排为主：加载或创建聚合、执行必要检查、调用领域行为、返回结果。聚合内部状态变化应由聚合方法完成。

### 修改已有聚合

修改已有聚合时，处理器通过仓储加载聚合，并调用聚合方法表达业务动作。

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

处理器不应直接设置 `order.Status`。支付状态的校验和流转由 `Order.Pay(...)` 维护。

### 创建新聚合

创建新聚合时，命令处理器负责准备用例所需的外部输入，调用聚合构造函数或工厂方法，并通过仓储添加聚合。聚合的初始状态和不变量应由聚合自身维护。

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

处理器不应在创建流程中绕过聚合构造函数或业务方法直接填充内部状态。

### 检查外部条件

需要库存、会员、优惠券、唯一性等外部数据参与判断时，处理器可以协调仓储、领域服务或应用服务。只依赖单个聚合内部状态的规则应放在聚合内部。

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

应用层可以协调多个依赖，但不应把聚合内部规则搬到处理器中。

## 查询

查询表示只读用例，不应修改领域状态。查询请求实现 `IQuery<TResult>`。

```csharp
public record GetOrderByIdQuery(Guid OrderId) : IQuery<OrderDto>;
```

查询处理器实现 `IQueryHandler<TQuery, TResult>`。处理器可以通过仓储读取聚合并映射为 DTO，也可以使用专门的读模型或查询服务。

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

复杂列表、分页统计和跨聚合展示通常更适合返回 DTO 或读模型，而不是返回聚合根。

## 验证

Dddify 默认启用 `ValidationBehavior<,>`，用于在处理器执行前运行 FluentValidation 验证器。验证失败时，管道会抛出 `ValidationException`。

启用 `AddApiResultWrapping()` 后，Dddify 会将 `ValidationException` 转换为统一 API 响应；未启用时，异常处理由项目自行负责。

### 验证器

验证器用于处理请求级规则，应继承 `AbstractValidator<TRequest>`：

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

### 规则边界

适合放在验证器中的规则：

- 必填。
- 字符串长度。
- 数字范围。
- 枚举取值。
- 集合不能为空。
- 请求格式约束。

不适合放在验证器中的规则：

- 聚合状态流转。
- 金额计算一致性。
- 支付后不可修改订单。
- 订单是否允许取消。

这些规则属于领域规则，应放在聚合、值对象或领域服务中。

## 应用异常

应用异常用于表达应用层在编排用例时发现的可预期失败。此类失败通常来自用例边界、访问控制或外部协作结果，而不是聚合内部不变量。项目可以定义具体异常并继承 `AppException`；`AppException` 继承自 `BusinessException`，会参与统一业务异常处理。

常见场景包括数据不存在、权限不足、重复提交、资源已被占用、外部服务返回业务失败等。

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

处理器应抛出语义明确的应用异常，避免使用 `null`、`false` 或通用异常表达业务失败：

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

应用异常的错误契约应面向接口调用方保持稳定：

- `WithErrorCode(...)`：设置稳定错误码，同时作为本地化资源 key，并支持格式化参数。
- `WithLogLevel(...)`：设置异常对应的日志级别。
- `WithMetadata(...)`：添加结构化诊断信息，例如业务对象标识、外部依赖返回码。

错误码和元数据约定：

- 使用稳定、语义明确的错误码。
- 错误码可用于 API 响应、日志检索和本地化资源匹配。
- `Metadata` 不应包含密码、令牌、身份证号等敏感信息。

启用 `AddApiResultWrapping()` 后，Dddify 会自动将应用异常包装为统一 API 响应；未启用时，异常处理、HTTP 状态码和响应格式由项目自行负责。

## DTO 与映射

DTO 定义应用层对外返回的数据形状，用于隔离领域对象和表现层模型。命令和查询表达用例输入，DTO 表达用例输出；映射代码负责在领域对象、读模型和 DTO 之间转换数据。

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

项目可以根据用例复杂度选择自动映射或手动映射。两种方式可以在同一项目中并存。

映射建议：

- 不将 EF Core entity 直接暴露给表现层。
- 不让 DTO 进入领域模型。
- 查询输出优先使用 DTO 或读模型。
- 命令输入可使用简单值、值对象或专用输入项类型。
- 只返回当前用例需要的字段。
- 明确处理枚举、值对象和时间格式的转换。
- 不返回敏感字段、内部状态字段或仅供持久化使用的字段。
- 复杂投影可提取为映射配置、私有方法、查询服务或读模型。
- 返回集合时固化结果，避免把延迟执行序列暴露给调用方。

### 自动映射

Dddify 在 `AddDddify()` 中内置 Mapster 集成，会扫描映射配置，并注册 `TypeAdapterConfig` 和 `IMapper`。自动映射适合字段结构稳定、转换规则简单的场景，可以减少重复赋值代码。

应用代码中建议注入 `IMapper` 执行映射，不建议直接使用 Mapster 的 `Adapt` 扩展方法。通过 `IMapper` 访问映射能力，可以保持依赖显式，便于测试替换，也能避免静态扩展方法分散在业务代码中。

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

涉及派生字段、值对象展开或集合项转换时，使用 `IRegister` 集中配置映射规则。

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

配置完成后，处理器仍只调用 `mapper.Map<OrderDto>(order)`；总额计算、状态转换和行项目金额展开由映射配置统一维护。

### 手动映射

手动映射的特点是映射逻辑显式、字段来源明确、转换规则可直接审查。DTO 或领域对象结构变化时，编译器更容易暴露不匹配问题。

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

## 工作单元管道 {#work-unit-pipeline}

启用 `AddDbContextWithUnitOfWork<TContext>(...)` 后，默认启用的 `UnitOfWorkBehavior<,>` 会在 MediatR 管道中处理命令请求。

`UnitOfWorkBehavior<,>` 只作用于 `ICommand` 和 `ICommand<TResult>`，不会处理 `IQuery<TResult>`。因为查询保持只读语义；若查询需要一致性读取，应由项目根据数据库和业务要求单独处理。

命令执行时，工作单元行为按以下规则处理：

- 当前没有事务时，开启新事务。
- 当前已有事务时，加入现有事务。
- 处理器执行成功后，调用 `IUnitOfWork.SaveChangesAsync(...)`。
- 若事务由当前行为开启，则提交事务。
- 若发生异常，回滚由当前行为开启的事务，并继续抛出原异常。

命令需要跳过工作单元管道时，可在命令类型上添加 `[SkipUnitOfWorkBehavior]`：

```csharp
[SkipUnitOfWorkBehavior]
public record RecalculateOrderReadModelCommand(Guid OrderId) : ICommand;
```

常见适用场景：

- 命令只触发内存计算、缓存刷新或本地文件处理，不修改数据库状态。
- 命令用于重建读模型、同步索引或发送已持久化的待处理任务，持久化边界由外部流程控制。
- 命令内部需要显式管理事务，或需要使用与默认 `IUnitOfWork` 不同的事务边界。

不应使用该属性绕过普通写用例的保存和事务处理。

## 领域事件处理

启用 `AddDbContextWithUnitOfWork<TContext>(...)` 后，Dddify 会注册 `DispatchDomainEventsInterceptor`，在 EF Core `SavingChanges` 阶段分发聚合收集的领域事件。拦截器会扫描当前 `DbContext` 跟踪的 `IAggregateRoot`，发布待处理的 `DomainEvents`，并清空事件集合。

领域事件通过 MediatR 在进程内发布。处理器实现 `IDomainEventHandler<TDomainEvent>`：

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

事件处理器会在当前 `SaveChanges` 继续执行前完成。若当前命令处于 `UnitOfWorkBehavior<,>` 管理的事务中，事件处理器中的本地写操作会参与同一工作单元和事务。

处理器职责建议：

- 可用于触发同一进程内的后续应用用例，例如创建发货单、记录通知任务或写入集成事件。
- 不用于修复聚合内部必须立即一致的状态；这类规则应在聚合方法中完成。
- 不建议在处理器中直接通过仓储修改其它聚合；需要执行写用例时，发送对应的 `Command`。
- 处理逻辑应保持短小，避免阻塞当前保存流程。

短信、邮件、Webhook 等外部服务调用不应直接放在同步领域事件处理器中执行。事件处理器通常仍处于当前保存流程和数据库事务边界内，直接调用外部服务会带来以下问题：

- 延长数据库事务时间。
- 外部服务失败可能回滚当前命令。
- 外部服务无法与本地数据库事务保证原子提交。
- 已发送的外部消息无法随事务回滚自动撤销。

推荐做法是在当前事务内只保存待处理记录，例如通知任务、发件箱记录或集成事件记录。事务提交后，再由后台任务读取记录并调用外部服务。

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

`CreateNotificationCommand` 负责保存待发送记录，不直接发送邮件。后台任务应在事务提交后执行实际投递，并记录发送状态、重试次数和错误信息。

实现后台投递时，建议遵循以下约定：

- 使用稳定的业务键或消息键保证幂等，避免重试导致重复发送。
- 对临时故障使用重试和退避策略，对永久失败记录失败状态。
- 记录处理状态，例如 `Pending`、`Processing`、`Succeeded`、`Failed`。
- 为 Webhook 或集成事件保留请求载荷、目标地址、响应状态和错误信息，便于排查。
- 发送逻辑与领域事件处理器解耦，不阻塞当前数据库事务。

只有在业务明确要求外部调用失败时回滚当前命令，且调用本身可控、快速、幂等时，才考虑在事件处理器中直接调用基础设施服务。

## 常见误区

- **处理器承载领域规则**：状态流转、金额计算和不变量校验散落在 handler 中。
- **查询修改状态**：`IQuery<TResult>` 处理器执行写操作，破坏读写语义和事务预期。
- **绕过聚合方法**：应用层直接设置聚合属性或内部集合，导致领域规则无法集中生效。
- **命令边界过大**：单个命令修改多个没有一致性关系的聚合，扩大事务范围和失败影响。
- **DTO 进入领域层**：将 DTO 当作领域对象使用，使表现层模型污染领域模型。
- **事件处理器承担外部投递**：同步发送短信、邮件或 Webhook，延长事务并导致不可回滚。

## 检查清单

- 命令是否只表达一个明确的写用例？
- 查询处理器是否保持只读，不修改持久化状态？
- handler 是否只负责编排：加载、检查、调用领域行为、返回结果？
- 聚合内部状态变化是否通过聚合方法完成？
- 请求格式、必填、长度等规则是否放在 FluentValidation 验证器中？
- 跨聚合或外部资源检查是否由应用层或领域服务协调？
- DTO 是否仅用于应用层输入输出，不进入领域模型？
- 写操作是否通过命令进入工作单元边界？
- 领域事件处理器是否避免修复聚合内部一致性？
- 外部副作用是否通过事务后后台流程处理？

