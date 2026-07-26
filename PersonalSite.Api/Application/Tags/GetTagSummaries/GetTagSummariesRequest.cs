namespace PersonalSite.Api.Application.Tags.GetTagSummaries;

public class GetTagSummariesRequest
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public string? Search { get; set; }
}
