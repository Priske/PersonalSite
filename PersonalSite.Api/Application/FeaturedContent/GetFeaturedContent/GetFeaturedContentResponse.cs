namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContent;

public record GetFeaturedContentResponse(
    IReadOnlyCollection<FeaturedContentResponse> Items);
