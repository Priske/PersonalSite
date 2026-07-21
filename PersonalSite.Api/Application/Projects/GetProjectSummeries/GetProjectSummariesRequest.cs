namespace PersonalSite.Api.Application.Projects.GetProjectSummeries;


public class GetProjectSummariesRequest
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public string? Search { get; set; }
}