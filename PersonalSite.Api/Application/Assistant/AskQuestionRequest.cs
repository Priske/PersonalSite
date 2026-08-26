namespace PersonalSite.Api.Application.Assistant;

public class AskQuestionRequest
{
    public const int MaximumQuestionLength = 1000;

    public required string Question { get; set; }
}