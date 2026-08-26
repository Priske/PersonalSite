using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Exceptions.Assistant;
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
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            throw new InvalidAssistantQuestionException(
                "A question is required.");
        }

        var question = request.Question.Trim();

        if (
            question.Length >
            AskQuestionRequest.MaximumQuestionLength)
        {
            throw new InvalidAssistantQuestionException(
                $"Questions cannot exceed " +
                $"{AskQuestionRequest.MaximumQuestionLength} characters.");
        }

        var knowledge = await knowledgeReader.ReadAsync(
            cancellationToken);

        var answer = await assistantClient.AskAsync(
            question,
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
                        question));

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