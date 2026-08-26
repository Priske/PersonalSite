using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Infrastructure.OpenAI;

namespace PersonalSite.Api.Application.Assistant;

public class AskQuestionCommandHandler(
    AssistantKnowledgeReader knowledgeReader,
    OpenAiAssistantClient assistantClient,
    ActivityTracker activityTracker) : IHandler
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

        await activityTracker.TrackAsync(
            ActivityType.AssistantChatLog,
            userId,
            metadata =>
            {
                metadata.Add(
                    "question",
                    new StringMetadataValue(
                        request.Question));

                metadata.Add(
                    "answer",
                    new StringMetadataValue(
                        answer));
            },
            cancellationToken);

        return new AskQuestionResponse
        {
            Answer = answer
        };
    }
}