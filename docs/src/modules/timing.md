# 时间与时区

Dddify 提供当前时间、当前时区和时间转换相关服务。该模块用于在应用层和基础设施层统一读取时间，并支持按当前执行上下文解析有效时区。

## 注册

在 `AddDddify` 中启用时间模块：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();
});
```

`AddTiming(...)` 会注册：

- `IClock`
- `ITimeZoneIdProvider`
- `ITimeZoneResolver`
- `ICurrentTimeZone`
- `IDateTimeConverter`

## IClock

`IClock` 用于读取当前时间。

```csharp
public interface IClock
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}
```

- `UtcNow` 返回当前 UTC 时间。
- `Now` 返回当前有效时区下的时间。

示例：

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

需要持久化的业务时间通常建议使用 `UtcNow`。面向展示或当前上下文的时间可以使用 `Now`。

## 当前时区

当前时区由 `ICurrentTimeZone` 表示：

```csharp
public interface ICurrentTimeZone
{
    TimeZoneInfo TimeZone { get; }
}
```

Dddify 通过 `ITimeZoneIdProvider` 获取候选时区 ID，再由 `ITimeZoneResolver` 解析为 `TimeZoneInfo`。

```csharp
public interface ITimeZoneIdProvider
{
    string? GetTimeZoneId();
}

public interface ITimeZoneResolver
{
    TimeZoneInfo Resolve(string? timeZoneId);
}
```

默认 `ITimeZoneIdProvider` 返回 `null`。`TimeZoneResolver` 在时区 ID 为空、无效或无法解析时回退到 UTC。

## 自定义时区来源

项目可以实现 `ITimeZoneIdProvider`，从请求头、用户设置、租户配置或其它上下文中提供时区 ID。

```csharp
public class HeaderTimeZoneIdProvider(IHttpContextAccessor httpContextAccessor)
    : ITimeZoneIdProvider
{
    public string? GetTimeZoneId()
    {
        return httpContextAccessor.HttpContext?
            .Request
            .Headers["X-Time-Zone"]
            .FirstOrDefault();
    }
}
```

注册自定义 provider：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming(options =>
    {
        options.UseTimeZoneIdProvider<HeaderTimeZoneIdProvider>();
    });
});
```

时区 ID 建议统一使用 IANA 标识，例如 `Asia/Shanghai`、`America/New_York`、`Europe/London`。Dddify 最终会通过 `TimeZoneInfo.FindSystemTimeZoneById(...)` 解析该值；部署前应确认目标运行环境能够解析项目使用的时区 ID。

## 时间转换

`IDateTimeConverter` 用于在 UTC、当前时区和指定时区之间转换 `DateTimeOffset`。

```csharp
public interface IDateTimeConverter
{
    DateTimeOffset ToCurrentTimeZone(DateTimeOffset value);
    DateTimeOffset ToUtc(DateTimeOffset value);
    DateTimeOffset ToTimeZone(DateTimeOffset value, TimeZoneInfo timeZone);
}
```

示例：

```csharp
public record TodoTimeDto(Guid Id, string Title, DateTimeOffset? CompletedAt);

public class GetTodoByIdQueryHandler(
    ITodoRepository todoRepository,
    IDateTimeConverter dateTimeConverter)
    : IQueryHandler<GetTodoByIdQuery, TodoTimeDto>
{
    public async Task<TodoTimeDto> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(query.Id, cancellationToken)
            ?? throw new TodoNotFoundException(query.Id);

        return new TodoTimeDto(
            todo.Id,
            todo.Title,
            todo.CompletedAt is null
                ? null
                : dateTimeConverter.ToCurrentTimeZone(todo.CompletedAt.Value));
    }
}
```

## 时区设计

应用应明确时间所属的业务时区。持久化时间建议使用 UTC，展示、输入解释和本地日期边界计算再转换到目标时区。

### 固定业务时区

单时区应用通常使用固定业务时区。该时区应通过 `ITimeZoneIdProvider` 显式提供，不应依赖服务器本地时区。

::: code-group

```csharp [FixedTimeZoneIdProvider.cs]
public class FixedTimeZoneIdProvider : ITimeZoneIdProvider
{
    public string? GetTimeZoneId() => "Asia/Shanghai";
}
```

```csharp [Program.cs]
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming(options =>
    {
        options.UseTimeZoneIdProvider<FixedTimeZoneIdProvider>();
    });
});
```

:::

以下场景应使用业务时区：

- 计算业务日、账期、营业日、结算日等本地日期边界。
- 展示时间、导出报表和生成通知内容。
- 解释用户输入的本地日期时间。

### 上下文时区

跨时区应用应将用户、租户或组织的首选 IANA 时区保存为业务配置，并在当前执行上下文中通过 `ITimeZoneIdProvider` 返回。

::: code-group

```csharp [UserTimeZoneIdProvider.cs]
public class UserTimeZoneIdProvider(ICurrentUser currentUser)
    : ITimeZoneIdProvider
{
    public string? GetTimeZoneId()
        => currentUser.IsAuthenticated
            ? currentUser.FindClaim("time_zone_id")
            : null;
}
```

```csharp [Program.cs]
builder.Services.AddDddify(cfg =>
{
    cfg.AddCurrentUser();

    cfg.AddTiming(options =>
    {
        options.UseTimeZoneIdProvider<UserTimeZoneIdProvider>();
    });
});
```

:::

### 时间存储

时间点表示确定的瞬间，例如订单支付时间、消息发送时间、会议开始时间。本地日历规则表示按指定时区解释的规则，例如每天 09:00 提醒、每月最后一个工作日结算、门店营业日。

持久化时间时，应按字段语义选择存储方式：

- 确定时间点：保存 UTC 时间，例如 `PaidAtUtc`、`CreatedAtUtc`、`StartsAtUtc`。使用 `DateTimeOffset` 时，也建议入库前转换为 UTC。
- 用户选择的未来本地时间：同时保存 UTC 时间和 IANA 时区 ID，例如 `StartsAtUtc` + `TimeZoneId`。
- 周期性本地规则：保存本地规则和时区，例如 `LocalTime` + `TimeZoneId` + 重复规则；生成具体实例时再换算为 UTC。
- 纯日期：使用 `DateOnly` 或等价类型，例如生日、账单日、营业日期。不要将本地日期保存为 UTC 零点。

### 本地时间输入

用户选择日期时间时，后端应使用明确的时区解释本地时间。时区可以来自当前用户、租户或组织上下文。当前端允许用户显式选择时区时，应随命令提交时区 ID。

```csharp
public record ScheduleTodoReminderCommand(
    Guid TodoId,
    DateOnly Date,
    TimeOnly Time) : ICommand;

public class ScheduleTodoReminderCommandHandler(
    ICurrentTimeZone currentTimeZone,
    ITodoRepository todoRepository)
    : ICommandHandler<ScheduleTodoReminderCommand>
{
    public async Task Handle(ScheduleTodoReminderCommand command, CancellationToken cancellationToken)
    {
        var todo = await todoRepository.GetAsync(command.TodoId, cancellationToken)
            ?? throw new TodoNotFoundException(command.TodoId);

        var timeZone = currentTimeZone.TimeZone;
        var localRemindAt = command.Date.ToDateTime(command.Time, DateTimeKind.Unspecified);

        if (timeZone.IsInvalidTime(localRemindAt))
        {
            throw new InvalidReminderTimeException(timeZone.Id, localRemindAt);
        }

        if (timeZone.IsAmbiguousTime(localRemindAt))
        {
            throw new AmbiguousReminderTimeException(timeZone.Id, localRemindAt);
        }

        var remindAtUtc = TimeZoneInfo.ConvertTimeToUtc(localRemindAt, timeZone);

        todo.ScheduleReminder(remindAtUtc, timeZone.Id);
    }
}
```

夏令时切换可能导致本地时间不存在或重复。应用应显式定义处理策略，例如拒绝输入、顺延到下一个有效时间，或要求用户选择重复时间中的具体实例。

### 时间回显

展示时间时，应将存储的 UTC 时间转换到目标时区。目标时区来源包括当前用户、租户、组织设置，或记录自身保存的 `TimeZoneId`。预约、会议、门店排班等场景，应优先使用记录保存的时区回显。

::: code-group

```csharp [GetMeetingQuery.cs]
public record GetMeetingQuery(Guid MeetingId) : IQuery<MeetingTimeDto>;

public class GetMeetingQueryHandler(
    IMeetingRepository meetingRepository,
    ITimeZoneResolver timeZoneResolver,
    IDateTimeConverter dateTimeConverter)
    : IQueryHandler<GetMeetingQuery, MeetingTimeDto>
{
    public async Task<MeetingTimeDto> Handle(GetMeetingQuery query, CancellationToken cancellationToken)
    {
        var meeting = await meetingRepository.GetAsync(query.MeetingId, cancellationToken)
            ?? throw new MeetingNotFoundException(query.MeetingId);

        var timeZone = timeZoneResolver.Resolve(meeting.TimeZoneId);

        return new MeetingTimeDto(
            dateTimeConverter.ToTimeZone(meeting.StartsAtUtc, timeZone),
            meeting.TimeZoneId);
    }
}
```

```csharp [MeetingTimeDto.cs]
public record MeetingTimeDto(DateTimeOffset StartsAt, string TimeZoneId);
```

:::

返回给客户端的数据建议包含带偏移量的时间和 IANA 时区 ID。带偏移量时间用于展示当前实例，时区 ID 用于后续编辑、重新选择时间和生成周期实例。

## 审计时间来源

启用 `AddDbContextWithUnitOfWork<TContext>(...)` 后，Dddify 会在保存实体时填充审计时间字段。审计时间来源由 `TimingOptions.AuditTimeSource` 控制：

- `UtcNow`：使用 `IClock.UtcNow`，即 UTC 时间。
- `Now`：使用 `IClock.Now`，即当前有效时区下的时间。

默认值为 `UtcNow`。审计字段通常建议使用 UTC，便于跨时区排序、筛选、日志关联和问题排查。展示给用户时，再通过 `IDateTimeConverter` 转换为目标时区。

仅当系统明确要求审计字段保存当前上下文时区时间时，才建议设置为 `AuditTimeSource.Now`：

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming(options =>
    {
        options.AuditTimeSource = AuditTimeSource.Now;
    });
});
```

## 使用建议

- 持久化时间优先使用 `IClock.UtcNow`。
- 展示时间或用户上下文时间使用 `IClock.Now` 或 `IDateTimeConverter`。
- 领域对象不应直接读取系统时间；需要时间时，由应用层通过参数传入。
- 时区 ID 建议统一使用 IANA 标识，并在部署环境中验证可解析性。
- 审计字段建议使用 UTC，除非项目明确要求记录当前上下文时区时间。