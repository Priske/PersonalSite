namespace PersonalSite.Api.Application.Tags.GetTagDetails;

public sealed record GetTagDetailsResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required IReadOnlyList<TagProjectResponse> Projects { get; init; }

    public required bool IsUsedByFeaturedContent { get; init; }

    public required string Source { get; init; }

    public int? CreatedByUserId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public int? LastEditedByUserId { get; init; }
    public DateTimeOffset LastEditedAt { get; init; }
}
