# Dependency Injection

Dddify is based on the built-in ASP.NET Core dependency injection container and uses Scrutor for convention-based registration. Dependency injection is responsible only for object creation and dependency composition. It should not contain business branches, workflow decisions, or domain rules.

## Principles

- Prefer constructor injection so type dependencies remain explicit.
- Prefer interfaces or stable abstractions to reduce coupling between modules.
- Manage service registration centrally and avoid scattered duplicate registrations in startup code.
- Do not inject `Scoped` services into `Singleton` services.
- When the same service is registered multiple times, the final resolution follows the registration order rules of ASP.NET Core DI.
- Business types should not use `IServiceProvider` to dynamically locate core dependencies.

## Convention-Based Registration

After `AddDddify()` is called, Dddify scans the configured assemblies and registers services by the following conventions:

- Types that implement `IDomainService`: registered as their own concrete type with `Scoped` lifetime.
- Repository implementations that implement `IRepository<TEntity, TKey>`: registered by matching interface with `Scoped` lifetime.
- Validators that implement `IValidator<T>`: registered as their implemented interfaces with `Scoped` lifetime.
- Types marked with dependency registration attributes: registered according to the lifetime and registration mode specified by the attribute.

## Dependency Markers

Regular application services, infrastructure services, and utility classes can declare their lifetime with dependency markers:

- `TransientDependencyAttribute`
- `ScopedDependencyAttribute`
- `SingletonDependencyAttribute`

Each marker must specify `RegistrationMode`, which controls how the service is exposed:

- `AsSelf`: registers only the concrete implementation type.
- `AsMatchingInterface`: registers the interface whose name matches the implementation type.
- `AsImplementedInterfaces`: registers all interfaces implemented by the type.

Use `AsSelf` when the service is used only inside the current module and does not need to be exposed through an interface:

```csharp
[TransientDependency(RegistrationMode.AsSelf)]
public class OrderPricingCalculator
{
    public decimal Calculate(decimal subtotal, decimal discount)
        => subtotal - discount;
}
```

Use `AsMatchingInterface` when the type and interface follow a one-to-one naming convention:

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

Use `AsImplementedInterfaces` when one implementation must be exposed through multiple interfaces:

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

Recommended lifetime choices:

| Lifetime | Suitable For |
| --- | --- |
| `ScopedDependency(...)` | Application or infrastructure services that depend on request-level objects such as repositories or the current user. |
| `TransientDependency(...)` | Lightweight, stateless services that can be created repeatedly, such as formatters and converters. |
| `SingletonDependency(...)` | Globally shared and thread-safe services, such as configuration providers or static rule caches. |

## Custom Scanning Registration

When a project needs additional batch registration rules, use `ConfigureScrutor(...)` to append Scrutor configuration. This method exposes Scrutor's `ITypeSourceSelector` configuration entry point and is suitable for registering services by naming convention, interface, attribute, or assembly.

The following example shows a common usage pattern. For specific scanning syntax, registration modes, lifetime configuration, and duplicate registration strategy, refer to the [Scrutor documentation](https://github.com/khellang/Scrutor).

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

Custom rules take effect together with Dddify's default rules. If the same service may be registered by multiple rules, define the registration order and expected resolution result according to Scrutor and ASP.NET Core DI registration rules.

## Explicit Registration

Not all services are suitable for attribute-based or scanning-based registration. Use the native .NET DI API for manual registration in the following scenarios:

- Third-party components or external SDKs.
- Services that must be created from configuration values.
- Services that must be created through factory methods.
- Services where a specific implementation must be selected explicitly.
- Special implementations that should not be affected by convention scanning.

```csharp
builder.Services.AddSingleton<IOrderCodeFormatter, DefaultOrderCodeFormatter>();

builder.Services.AddScoped<IMessageSender>(sp =>
{
    var options = sp.GetRequiredService<IOptions<MessageOptions>>().Value;
    return new EmailMessageSender(options.ConnectionString);
});
```

Place manual registrations in the startup configuration of the Web project or in service extension methods of the Infrastructure layer. Avoid scattering them across business types.

## Injection Style

Business code should prefer constructor injection. Dependencies appear explicitly in the type signature, allowing callers and tests to identify required collaborators directly.

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

Do not inject `IServiceProvider` into command handlers, domain services, or application services to dynamically locate dependencies. This hides real dependencies and may introduce lifetime issues.

`IServiceProvider` is more suitable for infrastructure scenarios:

- Factory objects create specific implementations based on conditions.
- Background jobs manually create service scopes.
- Middleware or framework extension points access request-level services.

If a business type requires many dependencies, first check whether its responsibility boundary is too broad instead of using `IServiceProvider` to hide constructor parameters.
