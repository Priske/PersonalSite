namespace PersonalSite.Api.Application.Analytics;

public sealed record DailyOutcome(DateOnly Date, int Successful, int Failed);
