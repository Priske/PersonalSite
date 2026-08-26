namespace PersonalSite.Api.Domain.Files;

public sealed class AssistantKnowledgeFile
{
    private AssistantKnowledgeFile() { }

    public AssistantKnowledgeFile(StoredFile file)
    {
        File = file;
    }

    public int AssistantKnowledgeId { get; private set; }
    public int StoredFileId { get; private set; }

    public StoredFile File { get; private set; } = null!;
}