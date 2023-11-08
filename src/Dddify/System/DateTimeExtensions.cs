namespace System;

/// <summary>
/// This class is used to provide <see cref="DateTime"/> type extension method.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Returns a formatted string, without hour, minute, and second. format: "yyyy-MM-dd".
    /// </summary>
    /// <param name="target">The DateTime to format.</param>
    /// <returns>Formatted string.</returns>
    public static string ToDateString(this DateTime target)
    {
        return target.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Returns a formatted string, without year, month, and day. format: "HH:mm:ss".
    /// </summary>
    /// <param name="target">The DateTime to format.</param>
    /// <returns>Formatted string.</returns>
    public static string ToTimeString(this DateTime target)
    {
        return target.ToString("HH:mm:ss");
    }

    /// <summary>
    /// Returns a formatted string, with milliseconds, format: "yyyy-MM-dd HH:mm:ss.fff".
    /// </summary>
    /// <param name="target">The DateTime to format.</param>
    /// <returns>Formatted string.</returns>
    public static string ToMillisecondString(this DateTime target)
    {
        return target.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }

    /// <summary>
    /// Gets the start DateTime of the current instance on the day.
    /// </summary>
    /// <param name="target">The DateTime to convert.</param>
    /// <returns>00:00:00</returns>
    public static DateTime StartTimeOfDay(this DateTime target)
    {
        return new DateTime(target.Year, target.Month, target.Day, 0, 0, 0);
    }

    /// <summary>
    /// Gets the end DateTime of the current instance on the day.
    /// </summary>
    /// <param name="target">The DateTime to convert.</param>
    /// <returns>23:59:59</returns>
    public static DateTime EndTimeOfDay(this DateTime target)
    {
        return new DateTime(target.Year, target.Month, target.Day, 23, 59, 59);
    }

    /// <summary>
    /// Gets the DateTime of the current instance on the first day of the week.
    /// </summary>
    /// <param name="target">The DateTime to convert.</param>
    /// <returns>The Monday.</returns>
    public static DateTime FirstDayOfWeek(this DateTime target)
    {
        var weeknow = Convert.ToInt32(target.DayOfWeek);
        weeknow = weeknow == 0 ? 7 - 1 : weeknow - 1;
        return target.AddDays(-1 * weeknow);
    }

    /// <summary>
    /// Gets the DateTime of the current instance on the last day of the week.
    /// </summary>
    /// <param name="target">The DateTime to convert.</param>
    /// <returns>The Sunday.</returns>
    public static DateTime LastDayOfWeek(this DateTime target)
    {
        var weeknow = Convert.ToInt32(target.DayOfWeek);
        weeknow = weeknow == 0 ? 7 : weeknow;
        return target.AddDays(7 - weeknow);
    }

    /// <summary>
    /// Gets the DateTime of the current instance on the first day of the month.
    /// </summary>
    /// <param name="target">The DateTime to convert.</param>
    /// <returns></returns>
    public static DateTime FirstDayOfMonth(this DateTime target)
    {
        return new DateTime(target.Year, target.Month, 1);
    }

    /// <summary>
    /// Gets the DateTime of the current instance on the last day of the month.
    /// </summary>
    /// <param name="target">The DateTime to convert.</param>
    /// <returns></returns>
    public static DateTime LastDayOfMonth(this DateTime target)
    {
        var firstDate = target.FirstDayOfMonth();
        return firstDate.AddMonths(1).AddDays(-1);
    }
}