using PersonalSite.Api.Domain.Assistant;

namespace PersonalSite.Api.Storage.Assistant;

public interface IAssistantKnowledgeRepository
{
    void Add(AssistantKnowledge knowledge);

    Task<AssistantKnowledge?> GetWithFilesAsync(
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
