namespace PersonalSite.Api.Application.Tags.GetTagDetails;

public sealed record GetTagDetailsResponse
{
    public required int Id { get; init; }

    public required string Name { get; init; }
    public required IReadOnlyList<TagProjectResponse> Projects { get; init; }


}