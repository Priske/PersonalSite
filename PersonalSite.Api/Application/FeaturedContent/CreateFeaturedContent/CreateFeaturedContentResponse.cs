namespace PersonalSite.Api.Application.FeaturedContent.CreateFeaturedContent;

public sealed class CreateFeaturedContentResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required IReadOnlyList<string> Tags { get; set; }
}
