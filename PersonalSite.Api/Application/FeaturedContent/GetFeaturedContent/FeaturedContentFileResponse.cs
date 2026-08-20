namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContent;

public record FeaturedContentFileResponse(
    int Id,
    string OriginalFileName,
    string ContentType,
    long SizeInBytes);
