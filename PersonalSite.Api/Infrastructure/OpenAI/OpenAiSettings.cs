namespace PersonalSite.Api.Infrastructure.OpenAI;

public sealed class OpenAiSettings
{
    public const string SectionName = "OpenAI";

    public required string ApiKey { get; init; }

    public required string Model { get; init; }
}