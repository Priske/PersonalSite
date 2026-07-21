using PersonalSite.Api.Domain.Common;
using PersonalSite.Api.Domain.Projects;

namespace PersonalSite.Api.Tests.Domain.Projects;

public sealed class ProjectTests
{
    [Fact]
    public void CanCreateProject()
    {
        var project = new Project
        {
            Id = 1,
            Title = new ProjectTitle("Personal Site"),
            Description = new ProjectDiscription(
                "A full-stack portfolio website."),
            RepositoryUrl = new Url(
                "https://github.com/johndoe/PersonalSite")
        };

        Assert.Equal(1, project.Id);
        Assert.Equal("Personal Site", project.Title.Value);
        Assert.Equal(
            "A full-stack portfolio website.",
            project.Description.Value);
        Assert.Equal(
            "https://github.com/johndoe/PersonalSite",
            project.RepositoryUrl.Value);

        Assert.Null(project.LiveUrl);
        Assert.False(project.IsFeatured);
        Assert.Equal(0, project.DisplayOrder);
    }
}