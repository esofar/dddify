# Getting Started

Dddify does not provide project templates or enforce a fixed application structure. This chapter shows how to create an ASP.NET Core Web API project from scratch and enable common Dddify capabilities.

## Prerequisites

Before you begin, make sure the local environment meets the following requirements:

- **Target framework**: Dddify targets .NET 10+. Use the .NET 10 SDK or a later version.
- **Development tools**: Visual Studio, Rider, or VS Code.
- **Basic knowledge**: Familiarity with the basic development workflow of ASP.NET Core and Entity Framework Core.
- **Database environment**: Prepare SQL Server, PostgreSQL, MySQL, or SQLite according to project requirements.
- **Network access**: Ensure NuGet is reachable for installing Dddify and related dependencies.

## Create the Solution

The recommended structure contains four projects: `Domain`, `Application`, `Infrastructure`, and `Web`.

- `Domain`: domain models, domain rules, domain events, and repository contracts.
- `Application`: commands, queries, validators, handlers, DTOs, and application orchestration.
- `Infrastructure`: EF Core, repository implementations, external service implementations, and infrastructure configuration.
- `Web`: HTTP entry points, startup configuration, and API models.

Use the `dotnet` CLI to create the solution, projects, and project references:

```bash
# 1. Create the solution
dotnet new sln -n MyCompany.MyProject --format slnx

# 2. Create the four projects
dotnet new classlib -n MyCompany.MyProject.Domain -o src/MyCompany.MyProject.Domain
dotnet new classlib -n MyCompany.MyProject.Application -o src/MyCompany.MyProject.Application
dotnet new classlib -n MyCompany.MyProject.Infrastructure -o src/MyCompany.MyProject.Infrastructure
dotnet new webapi -n MyCompany.MyProject.Web -o src/MyCompany.MyProject.Web

# 3. Add projects to the solution
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj

# 4. Add project references
dotnet add src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj reference src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj
dotnet add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj reference src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj
dotnet add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj reference src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj
dotnet add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj reference src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj
dotnet add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj reference src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj
```

Keep the reference direction stable: `Application` depends on `Domain`; `Infrastructure` implements abstractions defined by `Application` and `Domain`; `Web` acts as the composition root and references `Application` and `Infrastructure`.

## Install NuGet Packages

Install `Dddify` in the `Domain` project:

::: code-group

```bash [dotnet CLI]
dotnet add src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj package Dddify
```

```powershell [Package Manager Console]
Install-Package Dddify -ProjectName MyCompany.MyProject.Domain
```

:::

Install the EF Core database provider in the `Infrastructure` project. The following example uses SQLite:

::: code-group

```bash [dotnet CLI]
dotnet add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite
```

```powershell [Package Manager Console]
Install-Package Microsoft.EntityFrameworkCore.Sqlite -ProjectName MyCompany.MyProject.Infrastructure
```

:::

Install the EF Core tools package in the `Web` startup project to run migration and database update commands:

::: code-group

```bash [dotnet CLI]
dotnet add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj package Microsoft.EntityFrameworkCore.Tools
```

```powershell [Package Manager Console]
Install-Package Microsoft.EntityFrameworkCore.Tools -ProjectName MyCompany.MyProject.Web
```

:::

## Recommended Directory Structure

The following structure illustrates a common layered layout. Adjust it according to project size, team conventions, and business boundaries.

```text
├── MyCompany.MyProject.slnx                    # Solution file
├── src/
│   ├── MyCompany.MyProject.Domain/             # Domain layer
│   │   ├── Aggregates/                         # Aggregate roots, entities, value objects, and enumeration classes
│   │   ├── Events/                             # Domain events
│   │   ├── Exceptions/                         # Domain exceptions
│   │   ├── Services/                           # Domain services
│   │   ├── Repositories/                       # Repository contracts
│   │   └── Shared/                             # Shared models
│   ├── MyCompany.MyProject.Application/        # Application layer
│   │   ├── Commands/                           # Commands, validators, and handlers
│   │   ├── Queries/                            # Queries, validators, and handlers
│   │   ├── Events/                             # Domain event handlers
│   │   ├── Exceptions/                         # Application exceptions
│   │   ├── Services/                           # Application service abstractions
│   │   ├── Dtos/                               # Data transfer objects
│   │   ├── Mappings/                           # Mapster mapping configuration
│   │   └── Behaviors/                          # MediatR pipeline behaviors
│   ├── MyCompany.MyProject.Infrastructure/     # Infrastructure layer
│   │   ├── Data/                               # Persistence configuration
│   │   │   ├── Configurations/                 # EF Core entity type configurations
│   │   │   ├── Migrations/                     # EF Core migration files
│   │   │   └── ApplicationDbContext.cs         # EF Core context
│   │   ├── Repositories/                       # Repository implementations
│   │   ├── Services/                           # Infrastructure service implementations
│   │   ├── Jobs/                               # Background jobs or scheduled jobs
│   └── MyCompany.MyProject.Web/                # Presentation layer
│       ├── Controllers/                        # Controllers
│       ├── Models/                             # Request models
│       ├── Filters/                            # Filters
│       ├── Middlewares/                        # Middlewares
│       ├── Extensions/                         # Web-layer registration and pipeline extensions
│       └── Resources/                          # Localization resource files
└── tests/
    ├── MyCompany.MyProject.Domain.Tests/       # Domain layer tests
    ├── MyCompany.MyProject.Application.Tests/  # Application layer tests
    └── MyCompany.MyProject.IntegrationTests/   # Integration tests
```

## Create Global Usings

You can create `GlobalUsings.cs` in each project to centralize Dddify and common namespaces. Business files should still keep project-specific imports that are directly related to the current type.

::: code-group

```csharp [Domain]
global using Dddify.Exceptions;
global using Dddify.SharedKernel;
```

```csharp [Application]
global using Dddify.Exceptions;
global using Dddify.Messaging.Commands;
global using Dddify.Messaging.Events;
global using Dddify.Messaging.Queries;
global using FluentValidation;
global using MapsterMapper;
global using MediatR;
global using Microsoft.Extensions.Logging;
global using MyCompany.MyProject.Domain.Repositories;
```

```csharp [Infrastructure]
global using Dddify.Dependency;
global using Dddify.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Logging;
global using MyCompany.MyProject.Domain.Repositories;
global using MyCompany.MyProject.Infrastructure.Data;
```

```csharp [Web]
global using Dddify;
global using Dddify.AspNetCore.ResultWrapping;
global using MediatR;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;
global using MyCompany.MyProject.Infrastructure.Data;
```

:::

## Organize Layered Code

The following example uses the `Department` aggregate to show the basic code organization across the four projects.

### Domain Layer

The domain layer defines the aggregate root and repository contract. The aggregate maintains its own state and business invariants.

::: code-group

```csharp [Aggregates/Departments/Department.cs]
using MyCompany.MyProject.Domain.Exceptions.Departments;

namespace MyCompany.MyProject.Domain.Aggregates.Departments;

public class Department : AggregateRoot<Guid>
{
    public const int NameMaxLength = 100;

    private Department() { }

    public Department(Guid id, string name)
    {
        Id = id;
        Rename(name);
        IsActive = true;
    }

    public string Name { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    public void Rename(string name)
    {
        var normalizedName = name.Trim();

        Name = normalizedName;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            throw new DepartmentAlreadyDeactivatedException(Id);
        }

        IsActive = false;
    }
}
```

```csharp [Exceptions/Departments/DepartmentAlreadyDeactivatedException.cs]
namespace MyCompany.MyProject.Domain.Exceptions.Departments;

public class DepartmentAlreadyDeactivatedException : DomainException
{
    public DepartmentAlreadyDeactivatedException(Guid id)
    {
        WithErrorCode("department_already_deactivated");
        WithMetadata("DepartmentId", id);
    }
}
```

```csharp [Repositories/IDepartmentRepository.cs]
using MyCompany.MyProject.Domain.Aggregates.Departments;

namespace MyCompany.MyProject.Domain.Repositories;

public interface IDepartmentRepository : IRepository<Department, Guid>
{
}
```

:::

### Application Layer

The application layer orchestrates use cases and defines commands, queries, validators, handlers, DTOs, and application exceptions.

::: code-group

```csharp [Commands/Departments/CreateDepartmentCommand.cs]
using MyCompany.MyProject.Domain.Aggregates.Departments;

namespace MyCompany.MyProject.Application.Commands.Departments;

public sealed record CreateDepartmentCommand(string Name) : ICommand<Guid>;

public class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
{
    public CreateDepartmentCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(Department.NameMaxLength);
    }
}

public class CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository)
    : ICommandHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand command, CancellationToken cancellationToken)
    {
        var department = new Department(Guid.CreateVersion7(), command.Name);

        await departmentRepository.AddAsync(department, cancellationToken);

        return department.Id;
    }
}
```

```csharp [Queries/Departments/GetDepartmentByIdQuery.cs]
using MyCompany.MyProject.Application.Dtos.Departments;
using MyCompany.MyProject.Application.Exceptions.Departments;

namespace MyCompany.MyProject.Application.Queries.Departments;

public sealed record GetDepartmentByIdQuery(Guid Id) : IQuery<DepartmentDto>;

public class GetDepartmentByIdQueryHandler(IDepartmentRepository departmentRepository, IMapper mapper)
    : IQueryHandler<GetDepartmentByIdQuery, DepartmentDto>
{
    public async Task<DepartmentDto> Handle(GetDepartmentByIdQuery query, CancellationToken cancellationToken)
    {
        var department = await departmentRepository.GetAsync(query.Id, cancellationToken);

        return department is null
            ? throw new DepartmentNotFoundException(query.Id)
            : mapper.Map<DepartmentDto>(department);
    }
}
```

```csharp [Dtos/Departments/DepartmentDto.cs]
namespace MyCompany.MyProject.Application.Dtos.Departments;

public sealed record DepartmentDto(Guid Id, string Name, bool IsActive);
```

```csharp [Exceptions/Departments/DepartmentNotFoundException.cs]
using MyCompany.MyProject.Domain.Aggregates.Departments;

namespace MyCompany.MyProject.Application.Exceptions.Departments;

public class DepartmentNotFoundException : AppException
{
    public DepartmentNotFoundException(Guid id)
    {
        WithErrorCode("department_not_found");
        WithMetadata(nameof(Department.Id), id);
    }
}
```

:::

### Infrastructure Layer

The infrastructure layer implements technical details, including the EF Core context, entity configurations, repository implementations, and database migrations.

::: code-group

```csharp [Data/ApplicationDbContext.cs]
using MyCompany.MyProject.Domain.Aggregates.Departments;

namespace MyCompany.MyProject.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Department> Departments => Set<Department>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        modelBuilder.ApplyDefaultConventions();

        base.OnModelCreating(modelBuilder);
    }
}
```

```csharp [Data/Configurations/DepartmentConfiguration.cs]
using MyCompany.MyProject.Domain.Aggregates.Departments;

namespace MyCompany.MyProject.Infrastructure.Data.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(Department.NameMaxLength);

        builder.Property(x => x.IsActive)
            .IsRequired();
    }
}
```

```csharp [Repositories/DepartmentRepository.cs]
using MyCompany.MyProject.Domain.Aggregates.Departments;

namespace MyCompany.MyProject.Infrastructure.Repositories;

public class DepartmentRepository(ApplicationDbContext context)
    : RepositoryBase<ApplicationDbContext, Department, Guid>(context), IDepartmentRepository
{
}
```

:::

### Web Layer

The Web layer accepts HTTP requests and sends commands or queries through MediatR. Use cases are executed by the application layer.

::: code-group

```csharp [Controllers/DepartmentsController.cs]
using MyCompany.MyProject.Application.Commands.Departments;
using MyCompany.MyProject.Application.Dtos.Departments;
using MyCompany.MyProject.Application.Queries.Departments;
using MyCompany.MyProject.Web.Models.Departments;

namespace MyCompany.MyProject.Web.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController(ISender sender) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<ApiResult<Guid>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResultWithErrors>(StatusCodes.Status400BadRequest)]
    public async Task<Guid> Create([FromBody] CreateDepartmentRequest request, CancellationToken cancellationToken)
    {
        return await sender.Send(new CreateDepartmentCommand(request.Name), cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType<ApiResult<DepartmentDto>>(StatusCodes.Status200OK)]
    public async Task<DepartmentDto> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return await sender.Send(new GetDepartmentByIdQuery(id), cancellationToken);
    }
}
```

```csharp [Models/Departments/CreateDepartmentRequest.cs]
namespace MyCompany.MyProject.Web.Models.Departments;

public sealed record CreateDepartmentRequest(string Name);
```

```csharp [Program.cs]
builder.Services.AddControllers();

builder.Services.AddDddify(cfg =>
{
    cfg.AddApiResultWrapping();

    cfg.AddDbContextWithUnitOfWork<ApplicationDbContext>(options =>
    {
        options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
    });
});

var app = builder.Build();

app.MapControllers();

app.Run();
```

:::

Configure the database connection string in `appsettings.json` of the `Web` project:

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=MyProject.db"
  }
}
```

## Initialize the Database

Use EF Core to generate the initial migration and apply the model to the database. Before running migrations, confirm that the EF Core runtime, migration tools, and database provider versions are compatible.

::: code-group

```bash [dotnet ef]
dotnet ef migrations add InitialCreate --project src/MyCompany.MyProject.Infrastructure --startup-project src/MyCompany.MyProject.Web --context ApplicationDbContext --output-dir Data/Migrations
dotnet ef database update --project src/MyCompany.MyProject.Infrastructure --startup-project src/MyCompany.MyProject.Web --context ApplicationDbContext
```

```powershell [Package Manager Console]
Add-Migration InitialCreate -Project MyCompany.MyProject.Infrastructure -StartupProject MyCompany.MyProject.Web -Context ApplicationDbContext -OutputDir Data/Migrations
Update-Database -Project MyCompany.MyProject.Infrastructure -StartupProject MyCompany.MyProject.Web -Context ApplicationDbContext
```

:::

## Run the Application

```bash
dotnet run --project src/MyCompany.MyProject.Web
```
