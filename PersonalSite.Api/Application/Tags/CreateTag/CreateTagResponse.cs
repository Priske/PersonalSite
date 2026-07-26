namespace PersonalSite.Api.Application.Tags.CreateTag;

public sealed record CreateTagResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }
}