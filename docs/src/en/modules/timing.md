# Timing & Time Zones

Dddify provides services for current time, current timezone, and time conversion. This module is used to read time consistently in the application and infrastructure layers and supports resolving the effective timezone from the current execution context.

## Registration

Enable the timing module in `AddDddify`:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming();
});
```

`AddTiming(...)` registers:

- `IClock`
- `ITimeZoneIdProvider`
- `ITimeZoneResolver`
- `ICurrentTimeZone`
- `IDateTimeConverter`

## IClock

`IClock` is used to read the current time.

```csharp
public interface IClock
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}
```

- `UtcNow` returns the current UTC time.
- `Now` returns the time in the current effective timezone.

Example:

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

Business time that needs to be persisted usually should use `UtcNow`. Time used for display or current context can use `Now`.

## Current Time Zone

The current timezone is represented by `ICurrentTimeZone`:

```csharp
public interface ICurrentTimeZone
{
    TimeZoneInfo TimeZone { get; }
}
```

Dddify obtains a candidate timezone ID through `ITimeZoneIdProvider`, then resolves it to `TimeZoneInfo` through `ITimeZoneResolver`.

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

The default `ITimeZoneIdProvider` returns `null`. `TimeZoneResolver` falls back to UTC when the timezone ID is empty, invalid, or cannot be resolved.

## Custom Time Zone Source

Projects can implement `ITimeZoneIdProvider` to provide a timezone ID from request headers, user settings, tenant configuration, or other context.

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

Register the custom provider:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming(options =>
    {
        options.UseTimeZoneIdProvider<HeaderTimeZoneIdProvider>();
    });
});
```

Timezone IDs should use IANA identifiers consistently, such as `Asia/Shanghai`, `America/New_York`, and `Europe/London`. Dddify ultimately resolves the value through `TimeZoneInfo.FindSystemTimeZoneById(...)`; verify that the target runtime environment can resolve the timezone IDs used by the project before deployment.

## Time Conversion

`IDateTimeConverter` converts `DateTimeOffset` values between UTC, the current timezone, and a specified timezone.

```csharp
public interface IDateTimeConverter
{
    DateTimeOffset ToCurrentTimeZone(DateTimeOffset value);
    DateTimeOffset ToUtc(DateTimeOffset value);
    DateTimeOffset ToTimeZone(DateTimeOffset value, TimeZoneInfo timeZone);
}
```

Example:

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

## Time Zone Design

Applications should explicitly define the business timezone of each time value. Persisted time should use UTC; display, input interpretation, and local date boundary calculation should convert to the target timezone.

### Fixed Business Time Zone

Single-timezone applications usually use a fixed business timezone. This timezone should be provided explicitly through `ITimeZoneIdProvider` and should not depend on the server local timezone.

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

Use the business timezone in these scenarios:

- Calculating local date boundaries such as business day, accounting period, operating day, or settlement day.
- Displaying time, exporting reports, and generating notification content.
- Interpreting user-entered local date and time.

### Context Time Zone

Cross-timezone applications should save the preferred IANA timezone of the user, tenant, or organization as business configuration, and return it through `ITimeZoneIdProvider` in the current execution context.

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

### Time Storage

A point in time represents a definite instant, such as order payment time, message sending time, or meeting start time. Local calendar rules represent rules interpreted by a specified timezone, such as a daily 09:00 reminder, settlement on the last business day of each month, or a store operating day.

Choose the storage method according to field semantics:

- Definite instant: store UTC time, such as `PaidAtUtc`, `CreatedAtUtc`, and `StartsAtUtc`. When using `DateTimeOffset`, also convert to UTC before saving.
- Future local time selected by the user: store both UTC time and IANA timezone ID, such as `StartsAtUtc` + `TimeZoneId`.
- Recurring local rule: store local rule and timezone, such as `LocalTime` + `TimeZoneId` + recurrence rule; convert specific instances to UTC when generating them.
- Pure date: use `DateOnly` or an equivalent type, such as birthday, billing date, or operating date. Do not store a local date as UTC midnight.

### Local Time Input

When a user selects date and time, the backend should interpret the local time using an explicit timezone. The timezone can come from the current user, tenant, or organization context. When the frontend allows users to select a timezone explicitly, submit the timezone ID with the command.

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

Daylight saving time transitions may cause local times to be invalid or ambiguous. Applications should define an explicit handling strategy, such as rejecting the input, moving to the next valid time, or requiring the user to choose a specific instance of an ambiguous time.

### Time Display

When displaying time, convert stored UTC time to the target timezone. The target timezone can come from the current user, tenant, organization settings, or the `TimeZoneId` stored on the record itself. For appointments, meetings, store schedules, and similar scenarios, prefer the timezone stored on the record for display.

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

Data returned to clients should include both the time with offset and the IANA timezone ID. The time with offset is used to display the current instance; the timezone ID is used for later editing, time reselection, and generating recurring instances.

## Audit Time Source

After `AddDbContextWithUnitOfWork<TContext>(...)` is enabled, Dddify fills audit time fields when saving entities. The audit time source is controlled by `TimingOptions.AuditTimeSource`:

- `UtcNow`: uses `IClock.UtcNow`, which is UTC time.
- `Now`: uses `IClock.Now`, which is time in the current effective timezone.

The default value is `UtcNow`. Audit fields should usually use UTC to support sorting, filtering, log correlation, and troubleshooting across timezones. Convert them to the target timezone through `IDateTimeConverter` when displaying to users.

Set `AuditTimeSource.Now` only when the system explicitly requires audit fields to store the current context timezone time:

```csharp
builder.Services.AddDddify(cfg =>
{
    cfg.AddTiming(options =>
    {
        options.AuditTimeSource = AuditTimeSource.Now;
    });
});
```

## Recommendations

- Prefer `IClock.UtcNow` for persisted time.
- Use `IClock.Now` or `IDateTimeConverter` for display time or user-context time.
- Domain objects should not read system time directly; when time is required, pass it from the application layer as a parameter.
- Use IANA timezone identifiers consistently and verify that they can be resolved in the deployment environment.
- Prefer UTC for audit fields unless the project explicitly requires recording time in the current context timezone.
