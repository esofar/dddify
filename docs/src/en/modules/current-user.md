# Current User

Dddify provides a current user abstraction for reading user information from the current execution context in the application and infrastructure layers.

## Registration

Enable the current user module in `AddDddify`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser();
});
```

`AddCurrentUser(...)` registers:

- `IHttpContextAccessor`
- `ICurrentUserProvider`
- `ICurrentUser`

The default `ICurrentUserProvider` reads the current `ClaimsPrincipal` from `HttpContext.User`. When no `HttpContext` is available in non-HTTP contexts, it returns an empty `ClaimsPrincipal`.

## ICurrentUser

`ICurrentUser` is the main entry point for application code to read current user information.

```csharp
public interface ICurrentUser
{
    bool IsAuthenticated { get; }
    string Id { get; }
    IReadOnlyCollection<Claim> Claims { get; }
    string? FindClaim(string claimType);
}
```

The default user identifier claim type is `ClaimTypes.NameIdentifier`. If the claim is not found, `Id` returns an empty string. `IsAuthenticated` returns `true` only when the identity is authenticated and `Id` is not empty.

Example:

```csharp
public class CreateOrderCommandHandler(
    ICurrentUser currentUser,
    IOrderRepository orderRepository,
    IGuidGenerator guidGenerator)
    : ICommandHandler<CreateOrderCommand, Guid>
{
    public async Task<Guid> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var buyerId = currentUser.GetIdAsGuid();
        var order = new Order(guidGenerator.Create(), buyerId, command.ShippingAddress);

        await orderRepository.AddAsync(order, cancellationToken);

        return order.Id;
    }
}
```

The current user is execution-context information. Domain objects should not depend on `ICurrentUser` directly. When a user identifier is required, the application layer should read it and pass it to domain behavior as a parameter.

## Configure the User Identifier Claim

Use `UseIdClaim(...)` to specify the claim type used to resolve the user identifier. OIDC/JWT scenarios commonly use `sub`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser(options =>
    {
        options.UseIdClaim("sub");
    });
});
```

## Custom Provider

`ICurrentUserProvider` provides the `ClaimsPrincipal` for the current execution context:

```csharp
public interface ICurrentUserProvider
{
    ClaimsPrincipal GetCurrentUser();
}
```

Non-HTTP scenarios such as background jobs, message consumers, and scheduled tasks can replace the default provider.

```csharp
public class SystemCurrentUserProvider : ICurrentUserProvider
{
    public ClaimsPrincipal GetCurrentUser()
    {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "system")],
            authenticationType: "System");

        return new ClaimsPrincipal(identity);
    }
}
```

Register the custom provider:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser(options =>
    {
        options.UseProvider<SystemCurrentUserProvider>();
    });
});
```

## Extend Current User

When project-level information such as tenant, organization, or permission flags must be exposed, define an interface that inherits from `ICurrentUser` and register the additional service through `Enhanced<TAbstraction, TImplementation>()`.

```csharp
public interface IApplicationCurrentUser : ICurrentUser
{
    string? TenantId { get; }
}

public class ApplicationCurrentUser(ICurrentUser currentUser) : IApplicationCurrentUser
{
    public bool IsAuthenticated => currentUser.IsAuthenticated;

    public string Id => currentUser.Id;

    public IReadOnlyCollection<Claim> Claims => currentUser.Claims;

    public string? TenantId => FindClaim("tenant_id");

    public string? FindClaim(string claimType)
        => currentUser.FindClaim(claimType);
}
```

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser(options =>
    {
        options.Enhanced<IApplicationCurrentUser, ApplicationCurrentUser>();
    });
});
```

`Enhanced(...)` registers the specified interface in addition to the default `ICurrentUser`; it does not replace `ICurrentUser`.

## User Identifier Conversion

`CurrentUserExtensions` provides typed helper methods for reading the user identifier:

```csharp
Guid userId = currentUser.GetIdAsGuid();
long longId = currentUser.GetIdAsLong();
int intId = currentUser.GetIdAsInt();
```

These methods throw `InvalidOperationException` when `Id` is empty or its format does not match the target type. For anonymous access or system task scenarios, check `IsAuthenticated` first or provide an explicit user identifier through a custom `ICurrentUserProvider`.

## Recommendations

- Read the current user in the application layer and pass required identifiers into the domain model.
- Do not inject `ICurrentUser`, `IHttpContextAccessor`, or `ClaimsPrincipal` into domain objects.
- Background jobs and message consumers should use a custom `ICurrentUserProvider` to define the user context explicitly.
- Keep the user ID type consistent within the project and avoid mixing `Guid`, `long`, and `int` across use cases.
