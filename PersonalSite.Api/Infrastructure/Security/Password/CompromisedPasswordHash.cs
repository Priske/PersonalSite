namespace PersonalSite.Api.Infrastructure.Security.Password;

public sealed class CompromisedPasswordHash
{
    public required string Hash { get; init; }
    public long OccurrenceCount { get; init; }
}