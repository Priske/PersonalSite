using PersonalSite.Api.Domain.Assistant;
using Microsoft.EntityFrameworkCore;

namespace PersonalSite.Api.Storage.Assistant;

public sealed class EfAssistantKnowledgeRepository(
    AppDbContext dbContext)
    : IAssistantKnowledgeRepository
{
    public void Add(AssistantKnowledge knowledge)
    {
        dbContext.AssistantKnowledges.Add(knowledge);
    }

    public Task<AssistantKnowledge?> GetWithFilesAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.AssistantKnowledges
            .Include(knowledge => knowledge.Files)
            .ThenInclude(attachment => attachment.File)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
