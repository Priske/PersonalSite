namespace PersonalSite.Api.Application.Tags.CreateTag;

public sealed record CreateTagRequest
{
    public required string Name { get; init; }
}