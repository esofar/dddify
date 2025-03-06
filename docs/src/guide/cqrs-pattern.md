---
outline: [2, 2] 
---

# 命令与查询

Dddify 通过集成 MediatR 实现了 CQRS 模式，并且通过重新定义一组接口，清晰地划分了处理 *命令* 和 *查询* 的场景。

## 命令和命令处理器

*命令* 是引发状态改变的请求，通常与数据写操作相关。

**核心接口**

- `ICommand`：表示不返回结果的命令接口。
- `ICommandHandler<TCommand>`：表示处理不返回结果命令的处理器接口。
- `ICommand<TResult>`：表示返回结果的命令接口。
- `ICommandHandler<TCommand, TResult>`：表示处理返回结果命令的处理器接口。

**定义命令**

``` C#
public record CreateUserCommand() : ICommand<Guid>;
```

**定义命令处理器**

``` C#
public class CreateUserCommandHandler(IUserRepository userRepository) : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User(command.Name, command.Email);
        await userRepository.AddAsync(user);
        return user.Id;
    }
}
```

## 查询和查询处理器

以下是一组查询相关的接口。

- `IQuery<TResult>`：表示返回结果的查询接口。
- `IQueryHandler<TQuery, TResult>`：表示处理返回结果查询的处理器接口。

下面是一个示例，演示如何定义一个查询及查询处理器。

``` C#
public record GetUserByIdQuery(Guid UserId) : IQuery<UserDto>;

public class GetUserByIdQueryHandler(IUserRepository userRepository) : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(query.UserId);
        return user.Adapt<UserDto>();
    }
}
```

## 发送命令和查询

使用 `ISender` 或 `IMediator` 接口发送命令或查询。Dddify 会扫描应用所有项目程序集自动注册 MediatR 相关服务。

下面是一个示例，演示如何发送命令或查询。

``` C#
[ApiController]
public class UserController(ISender sender): ControllerBase  
{
    [HttpPost]
    public async Task<Guid> PostAsync([FromBody] CreateUserRequest request)
    {
        return await sender.Send(new CreateUserCommand(request.Name, request.Email));
    }

    [HttpGet("{id}")]
    public async Task<UserDto> GetAsync(Guid id)
    {
        return await sender.Send(new GetUserByIdQuery(id));
    }
}  
```

## 自动注册 MediatR 服务

Dddify 框架在启动时，会自动扫描您的应用程序程序集，查找所有实现了 ICommandHandler 和 IQueryHandler 接口的类，并将它们注册到 ASP.NET Core 的依赖注入容器中。这意味着您无需在 Startup.cs 或 Program.cs 中显式注册 MediatR 的服务，Dddify 已经为您完成了这项工作，简化了配置过程。


## 推荐命名规范

在命名 Query 和 Command 时，通常遵循以下规范：

- **清晰性**：名称应该清楚地表达出这个 Query 或 Command 的意图和功能。
- **简洁性**：在不牺牲可读性的前提下，尽量避免冗长的名称。
- **一致性**：整个项目中应遵循统一的命名规则，以便团队成员能够快速理解代码。

### Query 命名规范

1. **动词开头**：使用动词来描述操作，例如 `Get`、`Find`、`Search` 等。
2. **描述对象**：紧接动词之后描述操作的对象，例如 `User`、`Order`、`Product` 等。
3. **具体条件**：如果有特定条件，可以在对象后面添加条件描述，例如 `ById`、`ByName` 等。

示例：
- `GetUserByIdQuery`
- `FindOrdersByDateQuery`
- `SearchProductsByCategoryQuery`
- `GetUserDetailsWithPermissionsQuery`
- `FindActiveOrdersByCustomerIdQuery`
- `SearchProductsByCategoryAndPriceRangeQuery`

### Command 命名规范

1. **动词开头**：使用动词来描述操作，例如 `Create`、`Update`、`Delete` 等。
2. **描述对象**：紧接动词之后描述操作的对象，例如 `User`、`Order`、`Product` 等。
3. **具体条件**：如果有特定条件，可以在对象后面添加条件描述，例如 `WithDetails`、`ById` 等。

示例：
- `CreateUserCommand`
- `UpdateOrderStatusCommand`
- `DeleteProductByIdCommand`
- `CreateUserWithRolesCommand`
- `UpdateOrderStatusByCustomerIdCommand`
- `DeleteProductByIdAndCategoryCommand`