namespace PersonalSite.Api.Application.FeaturedContent.GetFeaturedContentDetails;

public sealed record FeaturedContentFileDetails(
    int Id,
    string OriginalFileName,
    string ContentType,
    long SizeInBytes);
