using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalSite.Api.Domain.Files;

namespace PersonalSite.Api.Storage.Configurations;

public sealed class AssistantKnowledgeFileConfiguration
    : IEntityTypeConfiguration<AssistantKnowledgeFile>
{
    public void Configure(
        EntityTypeBuilder<AssistantKnowledgeFile> builder)
    {
        builder.HasKey(file => new
        {
            file.AssistantKnowledgeId,
            file.StoredFileId
        });

        builder.HasOne(file => file.File)
            .WithMany()
            .HasForeignKey(file => file.StoredFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}