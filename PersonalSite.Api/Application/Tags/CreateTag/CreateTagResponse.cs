namespace PersonalSite.Api.Application.Tags.CreateTag;

public sealed record CreateTagResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Source { get; init; }

    public int? CreatedByUserId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public int? LastEditedByUserId { get; init; }

    public required DateTimeOffset LastEditedAt { get; init; }
}