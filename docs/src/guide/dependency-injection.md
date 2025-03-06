---
outline: [2, 4] 
---

# 依赖注入

Dddify 使用 ASP.NET Core 内置依赖注入容器，通过集成 [Scrutor](https://github.com/khellang/Scrutor) 提供更加灵活和友好的 API，进一步简化了服务注册工作。

## 服务注册

以下是三种服务注册方式，请根据适用场景选择，可混合使用多种方法实现最佳实践。

### 使用原生 API 手动注册

通过 `IServiceCollection` 接口直接注册服务，适用于少量或需要精细控制的场景。

**特点**

- 完全手动控制。
- 需要逐个注册服务，代码冗余度高。

**示例**

``` C#
builder.Services.AddTransient<IMyTransientService, MyTransientService>();
builder.Services.AddScoped<IMyScopedService, MyScopedService>();
builder.Services.AddSingleton<IMySingletonService, MySingletonService>();
```

更多高级用法请参考[官方文档](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-9.0)。

### 使用 Scrutor 批量注册

对于需要注册大量服务的场景，使用 Dddify 扩展的 `ScanFromProjectAssemblies` 或 Scrutor 提供的 `Scan` 方法，可以有效减少手动注册的工作量。

**特点**

- 减少重复代码。
- 支持通过命名约定、程序集扫描等方式批量注册。

**示例**

::: code-group
``` C# [方式一（推荐）]
builder.Services.AddDddify(cfg =>
{
    cfg.ScanFromProjectAssemblies(selector => selector // [!code focus]
        .AddClasses(classes => classes.AssignableTo<IMyTransientService>()) // [!code focus]
            .AsImplementedInterfaces() // [!code focus]
            .WithTransientLifetime()); // [!code focus]
});
```

``` C# [方式二]
builder.Services.Scan(scan => scan
     .FromAssemblyOf<Program>() // 扫描 Startup 类所在的程序集
        .AddClasses(classes => classes.AssignableTo<IMyTransientService>())
        .AsImplementedInterfaces()
        .WithTransientLifetime());
```
:::

::: info Dddify 为什么提供 `ScanFromProjectAssemblies`？
因为 Dddify 内部已扫描了启动应用所有*依赖项目程序集*，`ScanFromProjectAssemblies` 直接使用这些程序集进行服务注册，避免了重复加载程序集，减少反射开销，从而提升应用启动速度。
:::

### 基于 `Attribute` 自动注册

Dddify 提供了一种便捷的机制，通过自定义 *特性* 标记服务，并在应用程序启动时自动扫描和注册这些服务。适用于需要灵活控制服务注册的场景。

**特性介绍**

- `TransientDependencyAttribute`：将服务注册为 Transient 生命周期。
- `ScopedDependencyAttribute`：将服务注册为 Scoped 生命周期。
- `SingletonDependencyAttribute`：将服务注册为 Singleton 生命周期。

**注册类型**

以上特性均提供一个枚举类型参数 `RegistrationType` 来指定注册类型。支持以下选项：

- `AsSelf`: 将服务注册为其自身类型。
- `AsMatchingInterface`: 将服务注册为与其名称匹配的接口（例如 `MyService` -> `IMyService`）。
- `AsImplementedInterfaces`: 将服务注册为其实现的所有接口（未指定 `RegistrationType` 时默认使用此选项）。

**特点**

- 声明式注册，代码直观清晰。
- 特性区分生命周期，参数指定注册类型，灵活控制，覆盖常见场景。

**示例**

``` C#
// 注册为 Transient 服务，并注册到匹配的接口 IMyService
[TransientDependency(RegistrationType.AsMatchingInterface)]
public class MyService : IMyService { }

// 注册为 Scoped 服务，并注册为其自身类型 MyOtherService
[ScopedDependency(RegistrationType.AsSelf)] 
public class MyOtherService { }

// 注册为 Singleton 服务，默认注册到所有实现的接口
[SingletonDependency] 
public class MySingletonService : IMySingletonService { }
```

## 服务注入

在 ASP.NET Core 中，*构造函数注入* 和 *服务提供者* 是两种主要依赖解析方式，请理解它们之间的区别与特点，避免常见的误用和反模式。

### 构造函数注入

构造函数注入是 **最推荐**、**最常用** 的服务解析方式。它通过在类的构造函数中声明依赖项，让容器自动解析并注入这些依赖项。

**特点**

* **显式依赖**：类的依赖项在构造函数中一目了然，易于理解和维护。
* **强制依赖**：在类的构造函数中声明所需的依赖项，依赖关系在编译时明确。
* **可测试性**：由于依赖关系在构造函数中明确声明，易于进行单元测试。
* **推荐实践**：构造函数注入是依赖注入领域最广泛接受和推荐的方式。

**示例**

```C#
// 主构造函数注入依赖项
public class ProductService(ILogger<MyService> logger)
{
    public void GetProducts()
    {
        logger.LogInformation("获取产品信息...");
    }
}
```

### 服务提供者

`IServiceProvider` 是 ASP.NET Core 依赖注入容器的核心接口，通常被称为 *服务提供者*。它主要提供访问服务容器并解析已注册服务实例的能力。

**特点**

- **应用级根服务提供者**：`IServiceProvider` 是应用程序的根服务提供者，通常在应用程序启动时创建，并在整个应用程序生命周期内有效。
- **手动创建作用域**：要解析 Scoped 服务，需要手动创建一个新的作用域。可以通过调用 `CreateScope()` 方法来实现。
- **生命周期管理**：在手动创建的作用域内，服务的生命周期由作用域控制，确保 Scoped 服务在请求处理期间有效。

**示例**

``` C#
public class MyService(IServiceProvider serviceProvider)
{
    public void DoSomething()
    {
        // 手动创建作用域并解析服务，在 using 结束自动释放
        using var scope = serviceProvider.CreateScope();
        var scopedService = scope.ServiceProvider.GetRequiredService<IMyScopedService>();
        scopedService.PerformAction();
    }
}
```

`HttpContext.RequestServices` 是 `HttpContext` 对象的一个属性，它返回一个 **请求级别** 的 `IServiceProvider` 实例。HTTP 请求到达服务器时，ASP.NET Core 会创建一个新的、与该请求相关的服务容器，并将 `IServiceProvider` 实例存储在 `HttpContext.RequestServices` 中。

**特点**

- **请求级服务提供者**：`HttpContext.RequestServices` 是每个 HTTP 请求的服务提供者，生命周期与当前请求相同。
- **自动创建作用域**：框架为每个请求自动创建一个新的作用域，`RequestServices` 公开该请求范围的服务提供者。
- **生命周期管理**：在请求处理期间，所有 Scoped 服务都有效，确保服务在请求期间有效。

**示例**

``` C#
public class MyController : ControllerBase
{
    public IActionResult Get()
    {
        var scopedService = HttpContext.RequestServices.GetRequiredService<IMyScopedService>();
        scopedService.PerformAction();
        return Ok();
    }
}
```

### 最佳实践建议清单

1. **优先选择构造函数注入**：对于应用程序的大部分组件，构造函数注入都是首选的依赖注入方式。

2. **将 `IServiceProvider` 定位为框架基础设施工具**：它主要用于框架管道的特定环节和特殊的技术场景。

3. **应用启动配置文件中的 `IServiceProvider`**：在 `Program.cs` 中使用 `app.Services` 获取框架服务是合理且推荐的。

4. **框架扩展组件中的 `IServiceProvider` 或 `HttpContext.RequestServices`**：在中间件、授权策略提供程序等框架扩展组件中谨慎使用是可以接受的。

5. **工厂模式中谨慎使用 `IServiceProvider`**：实现工厂模式时可以考虑使用 `IServiceProvider`，但需要仔细评估其必要性，并优先考虑更符合依赖注入原则的替代方案。

6. **避免在业务逻辑类中滥用 `IServiceProvider`**：坚决避免在业务逻辑类中使用 `IServiceProvider` 动态查找依赖，这是 *服务定位器* 反模式。

7. **Controller Action 中谨慎使用 `HttpContext.RequestServices`**：优先使用构造函数注入。只有在极少数情况下，例如避免过度传递参数时，才考虑谨慎使用。

8. **业务逻辑组件不应依赖于 `HttpContext.RequestServices`**：保持业务逻辑组件与 HTTP 请求上下文的隔离。

9. **明确使用 `IServiceProvider` 的目的**：要清晰地了解使用 `IServiceProvider` 的目的和必要性，并避免为了“方便”而随意使用。

10. **避免在单例服务中直接解析作用域服务**：要特别小心避免在单例服务中直接从 `IServiceProvider` 解析作用域服务，这很可能导致生命周期管理混乱和潜在的 bug。