using PersonalSite.Api.Infrastructure.OpenAI;

namespace PersonalSite.Api.Application.Assistant;

public class AskQuestionCommandHandler(
    AssistantKnowledgeReader knowledgeReader,
    OpenAiAssistantClient assistantClient) : IHandler
{
    public async Task<AskQuestionResponse?> Execute(
        AskQuestionRequest request,
        int? userId,
        CancellationToken cancellationToken)
    {
        var knowledge = await knowledgeReader.ReadAsync(
            cancellationToken);

        var answer = await assistantClient.AskAsync(
            request.Question,
            knowledge,
            cancellationToken);

        return new AskQuestionResponse
        {
            Answer = answer
        };
    }
}