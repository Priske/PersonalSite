namespace PersonalSite.Api.Application.FeaturedContent.CreateFeaturedContent;

public sealed class CreateFeaturedContentRequest
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required IReadOnlyList<int> TagIds { get; set; }
}
