namespace PersonalSite.Api.Application.Users.GetUserSummeries;


public class GetUserSummariesRequest
{
    public int? Page { get; set; }

    public int? PageSize { get; set; }

    public string? Search { get; set; }
}