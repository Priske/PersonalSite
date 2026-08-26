using PersonalSite.Api.Domain.Exceptions.Assistant;
using PersonalSite.Api.Storage.Assistant;
using PersonalSite.Api.Storage.Files;

namespace PersonalSite.Api.Application.Assistant;

public sealed class AssistantKnowledgeReader(
    IAssistantKnowledgeRepository knowledgeRepository,
    IFileStorage fileStorage)
{
    public async Task<string> ReadAsync(
        CancellationToken cancellationToken)
    {
        var storedKnowledge =
            await knowledgeRepository.GetWithFilesAsync(
                cancellationToken);

        if (storedKnowledge is null ||
            storedKnowledge.Files.Count == 0)
        {
            throw new AssistantUnavailableException(
                "No assistant knowledge is configured.",
                new InvalidOperationException(
                    "AssistantKnowledge contains no files."));
        }

        var documents = new List<string>();

        foreach (var attachment in storedKnowledge.Files
                     .OrderBy(item => item.File.OriginalFileName))
        {
            var stream = await fileStorage.OpenReadAsync(
                attachment.File.StorageKey,
                cancellationToken);

            if (stream is null)
            {
                throw new AssistantUnavailableException(
                    "An assistant knowledge file is unavailable.",
                    new FileNotFoundException(
                        attachment.File.OriginalFileName));
            }

            await using (stream)
            {
                using var reader = new StreamReader(stream);

                var text = await reader.ReadToEndAsync(
                    cancellationToken);

                documents.Add(
                    $"# Document: {attachment.File.OriginalFileName}\n\n{text}");
            }
        }

        return string.Join(
            "\n\n---\n\n",
            documents);
    }
}