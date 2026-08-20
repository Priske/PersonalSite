using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Application.Skills.CreateSkill;
using PersonalSite.Api.Application.Skills.CreateSkillGroup;
using PersonalSite.Api.Application.Skills.UpdateSkill;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.Skills;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Skills.CreateSkillTests;

public sealed class CreateSkillTests : IntegrationTest
{
    [Fact]
    public async Task PostSkillAddsSkillAndAllowsReordering()
    {
        await AuthenticateAsUser(UserRole.Administrator);

        var groupResponse = await Client.PostAsJsonAsync(
            "/skill-groups",
            new CreateSkillGroupRequest
            {
                Name = "Backend",
                DisplayOrder = 1
            });

        var group = await groupResponse
            .ReadJsonAs<CreateSkillGroupResponse>(
                HttpStatusCode.Created);

        var firstResponse = await Client.PostAsJsonAsync(
            $"/skill-groups/{group.Id}/skills",
            new CreateSkillRequest
            {
                Name = "C#",
                DisplayOrder = 1
            });

        var first = await firstResponse
            .ReadJsonAs<CreateSkillResponse>(
                HttpStatusCode.Created);

        var staleResponse = await Client.PostAsJsonAsync(
            $"/skill-groups/{group.Id}/skills",
            new CreateSkillRequest
            {
                Name = "Existing temporary skill",
                DisplayOrder = 10_000
            });

        var stale = await staleResponse
            .ReadJsonAs<CreateSkillResponse>(
                HttpStatusCode.Created);

        var secondResponse = await Client.PostAsJsonAsync(
            $"/skill-groups/{group.Id}/skills",
            new CreateSkillRequest
            {
                Name = "ASP.NET Core",
                DisplayOrder = 10_000
            });

        var second = await secondResponse
            .ReadJsonAs<CreateSkillResponse>(
                HttpStatusCode.Created);

        Assert.Equal(10_001, second.DisplayOrder);

        var orderResponse = await Client.PutAsJsonAsync(
            $"/skill-groups/{group.Id}/skills/order",
            new UpdateSkillOrderRequest
            {
                SkillIds = [second.Id, first.Id, stale.Id]
            });

        await orderResponse.ShouldHaveStatusCode(
            HttpStatusCode.NoContent);

        var skills = Reader.Query(
            context => context.Skills
                .AsNoTracking()
                .Where(skill =>
                    skill.SkillGroupId == group.Id)
                .OrderBy(skill => skill.DisplayOrder)
                .ToList());

        Assert.Collection(
            skills,
            skill =>
            {
                Assert.Equal(second.Id, skill.Id);
                Assert.Equal(1, skill.DisplayOrder);
            },
            skill =>
            {
                Assert.Equal(first.Id, skill.Id);
                Assert.Equal(2, skill.DisplayOrder);
            },
            skill =>
            {
                Assert.Equal(stale.Id, skill.Id);
                Assert.Equal(3, skill.DisplayOrder);
            });

        Assert.All(
            skills,
            skill =>
            {
                Assert.Equal(
                    ContentSource.Official,
                    skill.Source);
                Assert.NotNull(skill.Created);
                Assert.NotNull(skill.Edited);
            });
    }
}
