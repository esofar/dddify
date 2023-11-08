namespace Dddify.Timing;

public class Clock : IClock
{
    private readonly DateTimeKind _dateTimeKind;

    public Clock(DateTimeKind dateTimeKind)
    {
        _dateTimeKind = dateTimeKind;
    }

    public virtual DateTime Now => _dateTimeKind == DateTimeKind.Utc ? DateTime.UtcNow : DateTime.Now;

    public virtual DateTimeKind Kind => _dateTimeKind;

    public virtual bool SupportsMultipleTimezone => _dateTimeKind == DateTimeKind.Utc;

    public virtual DateTime Normalize(DateTime dateTime)
    {
        if (Kind == DateTimeKind.Unspecified || Kind == dateTime.Kind)
        {
            return dateTime;
        }

        if (Kind == DateTimeKind.Local && dateTime.Kind == DateTimeKind.Utc)
        {
            return dateTime.ToLocalTime();
        }

        if (Kind == DateTimeKind.Utc && dateTime.Kind == DateTimeKind.Local)
        {
            return dateTime.ToUniversalTime();
        }

        return DateTime.SpecifyKind(dateTime, Kind);
    }
}