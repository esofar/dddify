# 当前用户

Dddify 提供了 `ICurrentUser` 接口，用于简化当前用户身份的获取和使用，业务代码无需关心具体实现，解耦业务逻辑与身份管理，增强代码可测试性。

## 接口定义

**基础属性**

`ICurrentUser` 接口定义了以下属性，用于获取当前用户的核心基本信息：

- `Principal`：提供当前用户的声明主体，此属性为 `ClaimsPrincipal?` 类型。
- `IsAuthenticated`：指示当前用户是否经过身份验证，此属性为 `bool` 类型。
- `Id`：获取当前用户的唯一标识符，此属性为 `Guid?` 类型。
- `Name`：获取当前用户的姓名，此属性为 `string?` 类型。
- `Roles`：获取当前用户的角色，此属性为 `IEnumerable<string>?` 类型。

**扩展方法**

`ICurrentUser` 提供了直接使用 `Principal` 声明的扩展方法：

- `GetAllClaims()`：获取所有的声明。
- `FindClaim(string claimType)`：查找指定类型声明。
- `FindClaims(string claimType)`：查找指定类型所有声明。
- `FindClaimValue(string claimType)`：查找指定类型声明值。
- `FindClaimValue<T>(string claimType)`：查找指定类型声明值，并转换数据类型。

## 实现 `ICurrentUser` 接口

定义一个类，实现 `ICurrentUser` 接口，并通过 `IHttpContextAccessor` 从 HTTP 请求上下文中获取当前用户信息。

示例：演示如何通过 `HttpContext` 获取当前用户信息。

``` C#
public class HttpContextUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public ClaimsPrincipal? Principal => httpContextAccessor.HttpContext?.User;
    public bool IsAuthenticated => Id.HasValue;
    public Guid? Id => this.FindClaimValue(ClaimTypes.NameIdentifier)?.ToGuid();
    public string? Name => this.FindClaimValue(ClaimTypes.Name);
    public IEnumerable<string>? Roles => this.FindClaims(ClaimTypes.Role).Select(c => c.Value);
}
```

## 注册服务

在 `Program.cs` 中注册 `ICurrentUser` 服务：

``` C#
builder.Services.AddHttpContextAccessor(); // [!code ++]

builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser<HttpContextUser>(); // [!code ++]
});
```
## 使用 `ICurrentUser`

通过依赖注入使用 `ICurrentUser` 接口来获取当前用户的信息。

示例：

``` C#
[ApiController]
public class UserController(ICurrentUser currentUser) : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetUserInfo()
    {
        if (!currentUser.IsAuthenticated)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            Id = currentUser.Id,
            Name = currentUser.Name,
            Roles = currentUser.Roles
        });
    }
}
```

## 扩展 `ICurrentUser`

根据项目需求，可以扩展 `ICurrentUser` 接口，添加更多属性或方法（如多租户、动态角色、自定义声明）。





下面是一个简单的示例，演示如何来扩展 `ICurrentUser` 接口。

首先，定义一个新的接口 `IExtendedCurrentUser`，继承 `ICurrentUser` 接口，并声明需要额外添加的用户属性。

``` C#

// 以下是一些常见的扩展场景：
public interface IExtendedCurrentUser : ICurrentUser
{
    // 扩展：获取用户邮箱
    string Email { get; }
    // 扩展：获取用户电话号码
    string PhoneNumber { get; }
    // 扩展：获取当前租户 ID
    Guid? TenantId { get; }
}
```

接下来，定义一个新的类 `ExtendedCurrentUser`，实现 `IExtendedCurrentUser` 接口，注入 `ICurrentUser` 继承原始属性，然后按业务需求实现额外添加的属性取值逻辑。

``` C# {8-9}
public class ExtendedCurrentUser(ICurrentUser currentUser) : IExtendedCurrentUser
{
    public Guid? Id => currentUser.Id;
    public string? Name => currentUser.Name;
    public ClaimsPrincipal? Principal => currentUser.Principal;
    public bool IsAuthenticated => currentUser.IsAuthenticated;

    public string Email => "johndoe@example.com"; // 示例邮箱
    public string Role => "Admin";  // 示例角色
}
```

最后，需要在应用启动时将扩展服务也注册到 DI 容器。打开 `Program.cs` 文件，进行如下代码调整：

``` C#
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser<HttpContextUser>(); // [!code --]
    cfg.AddCurrentUser<HttpContextUser, IExtendedCurrentUser, ExtendedCurrentUser>(); // [!code ++]
});
```

如此以来，可以通过注入 `IExtendedCurrentUser` 接口来访问当前用户。以下是一个简单的使用示例：

``` C#
public class AccountController(IExtendedCurrentUser currentUser) : ControllerBase
{
    public ActionResult GetUserInfo()
    {
        Console.WriteLine($"Email: {currentUser.Email}"); // 输出: "johndoe@example.com"
        Console.WriteLine($"Role: {currentUser.Role}"); // 输出: "Admin"
    }
}
```