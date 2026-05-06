# Dddify

Dddify is a lightweight DDD integration framework for modern ASP.NET Core applications.

It provides reusable building blocks for domain modeling, application orchestration, messaging, validation, dependency registration, timing, localization, API result wrapping, and Entity Framework Core persistence integration.

## Installation

```bash
dotnet add package Dddify
```

## Capabilities

- Domain primitives: aggregate roots, entities, value objects, domain events, auditing contracts, soft delete, and concurrency stamp support.
- Application messages: commands, queries, handlers, validators, and domain event handlers based on MediatR.
- Dependency registration: convention-based registration through Scrutor and explicit dependency markers.
- EF Core integration: DbContext registration, unit of work, repository base class, model conventions, save interceptors, and domain event dispatching.
- ASP.NET Core integration: current user access, JSON localization, API result wrapping, and exception response mapping.
- Timing support: current time, current timezone, timezone resolution, time conversion, and audit time source configuration.

## Quick Start

Register Dddify in the ASP.NET Core startup code:

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();
    cfg.AddCurrentUser();
    cfg.AddLocalization();
    cfg.AddApiResultWrapping();

    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});
```

## Example Command

```csharp
public sealed record CreateTodoCommand(string Title) : ICommand<Guid>;

public sealed class CreateTodoCommandHandler(ITodoRepository todoRepository)
    : ICommandHandler<CreateTodoCommand, Guid>
{
    public async Task<Guid> Handle(CreateTodoCommand command, CancellationToken cancellationToken)
    {
        var todo = new Todo(Guid.NewGuid(), command.Title);

        await todoRepository.AddAsync(todo, cancellationToken);

        return todo.Id;
    }
}
```

## Links

- Documentation: https://dddify.net
- Source code: https://github.com/esofar/dddify
