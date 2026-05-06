using Dddify.Timing;

namespace TodoApp.Infrastructure.Services;

public class FixedTimeZoneIdProvider : ITimeZoneIdProvider
{
    public string? GetTimeZoneId() => "Asia/Shanghai";
}