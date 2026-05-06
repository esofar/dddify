# Configuration

`AddDddify()` is the unified registration entry point for Dddify. It registers core services and enables optional modules according to configuration.

```csharp
builder.Services.AddDddify(cfg =>
{
    // Configure Dddify modules and extension points.
});
```

## Core Registration

`AddDddify()` registers Dddify's basic capabilities, including type scanning, handler registration, validator registration, mapping configuration, and convention-based dependency injection.

Default registrations:

- MediatR: scans command, query, and domain event handlers.
- FluentValidation: scans and registers validators.
- Mapster: scans mapping configuration and registers `TypeAdapterConfig` and `IMapper`.
- Scrutor: registers domain services, repository implementations, and services with dependency markers by convention.

Default pipeline behaviors:

- `ValidationBehavior<,>`: runs the FluentValidation validators for the current request type.
- `UnitOfWorkBehavior<,>`: executes unit-of-work logic around command requests.

In most cases, keep the default pipeline behaviors enabled. Disable a pipeline behavior only when the project needs full control over validation flow, save timing, or transaction boundaries:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.DisableValidationBehavior();
    cfg.DisableUnitOfWorkBehavior();
});
```

After disabling a behavior, it will not be added to the MediatR pipeline. The project must then keep validation, saving, and transaction handling consistent on its own.

## Assembly Scanning

By default, Dddify uses `DependencyContext.Default` to resolve the project assemblies referenced by the current application. These assemblies are used for Mapster, MediatR, FluentValidation, and Scrutor scanning. Specify assemblies explicitly when you need precise control over the scan scope:

```csharp
cfg.ScanAssemblies(
    typeof(ApplicationMarker).Assembly,
    typeof(DomainAssemblyMarker).Assembly,
    typeof(InfrastructureMarker).Assembly,
    typeof(Program).Assembly);
```

After assemblies are specified explicitly, Dddify scans only the provided assemblies. If a handler, validator, mapping configuration, or convention-registered service is outside the scan scope, it will not be registered automatically.

## Optional Modules

Core registration only includes common pipelines and convention-based service registration. Capabilities such as timing, current user, localization, API result wrapping, and EF Core integration must be enabled explicitly according to project requirements.

Optional modules are registered through the configuration delegate of `AddDddify`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();
    cfg.AddCurrentUser();
    cfg.AddApiResultWrapping();
});
```

Common modules include:

- [`AddTiming(...)`](/en/modules/timing): when the project needs unified access to the current time, current timezone, or time conversion.
- [`AddCurrentUser(...)`](/en/modules/current-user): when the application or infrastructure layer needs current user information.
- [`AddLocalization(...)`](/en/modules/localization): when the project needs JSON localization or localized error messages.
- [`AddApiResultWrapping(...)`](/en/modules/api-result-wrapping): when the project needs a unified API response structure and exception response mapping.
- [`AddDbContextWithUnitOfWork<TContext>(...)`](/en/modules/entity-framework-core): when the project uses EF Core persistence integration.

Optional modules can be enabled independently or combined. See the corresponding module documentation for configuration options and usage.

## Extension Configuration

Dddify provides configuration extension points for MediatR, Scrutor, and Mapster. Project-level configuration can be added while keeping the default registration conventions:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.ConfigureMediatR(options =>
    {
        // Configure MediatRServiceConfiguration.
    });

    cfg.ConfigureScrutor(selector =>
    {
        // Add extra Scrutor scanning rules.
    });

    cfg.ConfigureMapster(config =>
    {
        // Customize TypeAdapterConfig.
    });
});
```

Extension configuration takes effect together with Dddify's default configuration. When adding custom scanning rules, avoid unnecessary duplicate registrations with existing registrations.