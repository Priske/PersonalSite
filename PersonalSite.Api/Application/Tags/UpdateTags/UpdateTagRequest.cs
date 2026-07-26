namespace PersonalSite.Api.Application.Tags.UpdateTags;

public sealed record UpdateTagRequest
{
    public required int Id { get; init; }

    public required string Name { get; init; }
};