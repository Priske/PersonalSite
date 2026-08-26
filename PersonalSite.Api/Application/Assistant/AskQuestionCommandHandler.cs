using System.Text.Json;
using PersonalSite.Api.Application.Projects.GetProjectSummeries;
using PersonalSite.Api.Infrastructure.OpenAI;

namespace PersonalSite.Api.Application.Assistant;

public class AskQuestionCommandHandler(
    GetProjectSummeriesQueryHandler getProjectsHandler,
    OpenAiAssistantClient assistantClient) : IHandler
{
    public async Task<AskQuestionResponse?> Execute(
        AskQuestionRequest request,
        int? userId,
        CancellationToken cancellationToken)
    {
        var answer = await assistantClient.AskAsync(
            request.Question,
            async (search, token) =>
            {
                var projects = await getProjectsHandler.Execute(
                    new GetProjectSummariesRequest
                    {
                        Page = 1,
                        PageSize = 10,
                        Search = search
                    },
                    token);

                return JsonSerializer.Serialize(projects.Items);
            },
            cancellationToken);

        return new AskQuestionResponse
        {
            Answer = answer
        };
    }
}