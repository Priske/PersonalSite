using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Domain.Assistant;

public sealed class AssistantKnowledge
{
    private AssistantKnowledge() { }

    public int Id { get; private set; }

    public ICollection<AssistantKnowledgeFile> Files { get; private set; } = [];

    public static AssistantKnowledge Create()
    {
        return new AssistantKnowledge();
    }
}