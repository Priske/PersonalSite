using PersonalSite.Api.Application.Tags.GetTagSummaries;

namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContentDetails;

public sealed class GetFeaturedContentDetailsResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required IReadOnlyList<FeaturedContentFileDetails> Files { get; set; }
    public required IReadOnlyList<TagSummary> Tags { get; set; }
}
