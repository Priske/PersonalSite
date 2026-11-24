namespace PersonalSite.Api.Application.Analytics;

public static class AnalyticsCalendar
{
    public static TimeZoneInfo Resolve(string? timeZone) =>
        timeZone is null ? TimeZoneInfo.Utc : TimeZoneInfo.FindSystemTimeZoneById(timeZone);

    public static DateOnly Date(DateTimeOffset timestamp, TimeZoneInfo zone) =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(timestamp, zone).DateTime);

    public static IReadOnlyList<DailyCount> Counts(IEnumerable<DateTimeOffset> timestamps, string? timeZone)
    {
        var zone = Resolve(timeZone);
        return timestamps.GroupBy(timestamp => Date(timestamp, zone))
            .OrderBy(group => group.Key)
            .Select(group => new DailyCount(group.Key, group.Count())).ToList();
    }

    public static IReadOnlyList<DailyOutcome> Outcomes(
        IEnumerable<(DateTimeOffset Timestamp, bool Successful)> events, string? timeZone)
    {
        var zone = Resolve(timeZone);
        return events.GroupBy(item => Date(item.Timestamp, zone))
            .OrderBy(group => group.Key)
            .Select(group => new DailyOutcome(group.Key,
                group.Count(item => item.Successful), group.Count(item => !item.Successful))).ToList();
    }
}
