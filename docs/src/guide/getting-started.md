# 快速开始

Dddify 不提供项目模板，也不强制应用结构。本章演示如何从零创建一个 ASP.NET Core Web API 项目，并接入 Dddify 的常用能力。

## 必要条件

开始前，请确认本地环境满足以下要求：

- **目标框架**：Dddify 面向 .NET 10+，请使用 .NET 10 SDK 或更高版本。
- **开发工具**：Visual Studio、Rider 或 VS Code。
- **基础知识**：熟悉 ASP.NET Core 和 Entity Framework Core 的基本开发流程。
- **数据库环境**：根据项目需要准备 SQL Server、PostgreSQL、MySQL 或 SQLite。
- **网络访问**：确保可访问 NuGet，用于安装 Dddify 及相关依赖包。

## 创建解决方案

推荐按 `Domain`、`Application`、`Infrastructure`、`Web` 四层组织项目：

- `Domain`：领域模型、领域规则、领域事件和仓储契约。
- `Application`：命令、查询、验证器、处理器、DTO 和应用编排。
- `Infrastructure`：EF Core、仓储实现、外部服务实现和基础设施配置。
- `Web`：HTTP 入口、启动配置和接口模型。

使用 `dotnet` CLI 创建解决方案、项目和项目引用：

```bash
# 1. 创建解决方案
dotnet new sln -n MyCompany.MyProject --format slnx

# 2. 创建四层项目
dotnet new classlib -n MyCompany.MyProject.Domain -o src/MyCompany.MyProject.Domain
dotnet new classlib -n MyCompany.MyProject.Application -o src/MyCompany.MyProject.Application
dotnet new classlib -n MyCompany.MyProject.Infrastructure -o src/MyCompany.MyProject.Infrastructure
dotnet new webapi -n MyCompany.MyProject.Web -o src/MyCompany.MyProject.Web

# 3. 添加项目到解决方案
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj
dotnet sln MyCompany.MyProject.slnx add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj

# 4. 添加项目引用
dotnet add src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj reference src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj
dotnet add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj reference src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj
dotnet add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj reference src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj
dotnet add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj reference src/MyCompany.MyProject.Application/MyCompany.MyProject.Application.csproj
dotnet add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj reference src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj
```

项目引用方向应保持稳定：`Application` 依赖 `Domain`；`Infrastructure` 实现 `Application` 和 `Domain` 定义的抽象；`Web` 作为组合根引用 `Application` 和 `Infrastructure`。

## 安装 NuGet 包

在 `Domain` 项目中安装 `Dddify`：

::: code-group

```bash [dotnet CLI]
dotnet add src/MyCompany.MyProject.Domain/MyCompany.MyProject.Domain.csproj package Dddify
```

```powershell [Package Manager Console]
Install-Package Dddify -ProjectName MyCompany.MyProject.Domain
```

:::

在 `Infrastructure` 项目中安装 EF Core 数据库提供程序。以下示例使用 SQLite：

::: code-group

```bash [dotnet CLI]
dotnet add src/MyCompany.MyProject.Infrastructure/MyCompany.MyProject.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite
```

```powershell [Package Manager Console]
Install-Package Microsoft.EntityFrameworkCore.Sqlite -ProjectName MyCompany.MyProject.Infrastructure
```

:::

在 `Web` 启动项目中安装 EF Core 工具包，用于执行迁移和数据库更新命令：

::: code-group

```bash [dotnet CLI]
dotnet add src/MyCompany.MyProject.Web/MyCompany.MyProject.Web.csproj package Microsoft.EntityFrameworkCore.Tools
```

```powershell [Package Manager Console]
Install-Package Microsoft.EntityFrameworkCore.Tools -ProjectName MyCompany.MyProject.Web
```

:::

## 推荐目录结构

以下结构用于说明常见分层方式。实际项目可以根据规模、团队约定和业务边界调整。

```text
├── MyCompany.MyProject.slnx                    # 解决方案文件
├── src/
│   ├── MyCompany.MyProject.Domain/             # 领域层
│   │   ├── Aggregates/                         # 聚合根、实体、值对象、枚举类
│   │   ├── Events/                             # 领域事件
│   │   ├── Exceptions/                         # 领域异常
│   │   ├── Services/                           # 领域服务
│   │   ├── Repositories/                       # 仓储契约
│   │   └── Shared/                             # 共享模型
│   ├── MyCompany.MyProject.Application/        # 应用层
│   │   ├── Commands/                           # 命令、验证器和处理器
│   │   ├── Queries/                            # 查询、验证器和处理器
│   │   ├── Events/                             # 领域事件处理器
│   │   ├── Exceptions/                         # 应用异常
│   │   ├── Services/                           # 应用层服务抽象
│   │   ├── Dtos/                               # 数据传输对象
│   │   ├── Mappings/                           # Mapster 映射配置
│   │   └── Behaviors/                          # MediatR 管道行为
│   ├── MyCompany.MyProject.Infrastructure/     # 基础设施层
│   │   ├── Data/                               # 持久化配置
│   │   │   ├── Configurations/                 # EF Core 实体类型配置
│   │   │   ├── Migrations/                     # EF Core 数据库迁移文件
│   │   │   └── ApplicationDbContext.cs         # EF Core 上下文
│   │   ├── Repositories/                       # 仓储实现
│   │   ├── Services/                           # 基础服务实现
│   │   ├── Jobs/                               # 后台任务或定时任务
│   └── MyCompany.MyProject.Web/                # 表现层
│       ├── Controllers/                        # 控制器
│       ├── Models/                             # 请求模型
│       ├── Filters/                            # 过滤器
│       ├── Middlewares/                        # 中间件
│       ├── Extensions/                         # Web 层注册和管道扩展
│       └── Resources/                          # 本地化资源文件
└── tests/
    ├── MyCompany.MyProject.Domain.Tests/       # 领域层测试
    ├── MyCompany.MyProject.Application.Tests/  # 应用层测试
    └── MyCompany.MyProject.IntegrationTests/   # 集成测试
```

## 创建全局引用

可以在各项目中创建 `GlobalUsings.cs`，集中放置 Dddify 和常用命名空间。业务文件仍应保留与当前类型直接相关的项目内引用。

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

## 组织分层代码

下面以 `Department` 聚合为例，说明四层项目的基本代码组织方式。

### Domain 层

领域层定义聚合根和仓储契约。聚合负责维护自身状态和业务不变量。

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

### Application 层

应用层负责编排用例，定义命令、查询、验证器、处理器、DTO 和应用异常。

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

### Infrastructure 层

基础设施层实现技术细节，包含 EF Core 上下文、实体配置、仓储实现和数据库迁移。

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

### Web 层

Web 层承接 HTTP 请求，通过 MediatR 发送命令和查询，由应用层执行用例。

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

在 `Web` 项目的 `appsettings.json` 中配置数据库连接字符串：

```json
{
  "ConnectionStrings": {
    "Default": "Data Source=MyProject.db"
  }
}
```

## 初始化数据库

使用 EF Core 生成初始迁移，并将模型应用到数据库。执行迁移前，应确认 EF Core 运行时、迁移工具和数据库 Provider 版本兼容。

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

## 运行应用程序

```bash
dotnet run --project src/MyCompany.MyProject.Web
```

