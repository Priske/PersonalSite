namespace PersonalSite.Api.Application.Analytics;

public sealed record DailyVideo(DateOnly Date, int Plays, int Completions, decimal WatchedSeconds);
