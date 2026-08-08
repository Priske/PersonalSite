namespace PersonalSite.Api.Application.Tags.GetTagSummaries;

public class TagSummary
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Source { get; init; }

    public int? CreatedByUserId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public int? LastEditedByUserId { get; init; }
    public DateTimeOffset LastEditedAt { get; init; }
}