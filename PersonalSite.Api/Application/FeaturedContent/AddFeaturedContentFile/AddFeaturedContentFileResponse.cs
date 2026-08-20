namespace PersonalSite.Api.Application.FeaturedContent.AddFeaturedContentFile;

public record AddFeaturedContentFileResponse(
    int Id,
    string OriginalFileName,
    string ContentType,
    long SizeInBytes);