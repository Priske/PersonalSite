using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Tags;
using PersonalSite.Api.Storage.Tags;

namespace PersonalSite.Api.Application.Tags.CreateTag;

public sealed class CreateTagCommandHandler(ITagRepository tagRepository) : IHandler
{
    public async Task<CreateTagResponse> Execute(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var name = new TagName(request.Name);

        if (await tagRepository.TagExistsAsync(name))
        {
            throw new DomainException("A tag with this name already exists.");
        }

        var tag = new Tag
        {
            Name = name
        };

        await tagRepository.AddAsync(tag, cancellationToken);

        return new CreateTagResponse
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}