namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContent;

public record FeaturedContentResponse(
    int Id,
    string Title,
    string Description,
    IReadOnlyCollection<FeaturedContentFileResponse> Files,
    IReadOnlyCollection<string> Tags);
