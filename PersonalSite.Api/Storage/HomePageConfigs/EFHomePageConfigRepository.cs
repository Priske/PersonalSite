using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.HomePageConfigs;

public class EfHomePageRepository(AppDbContext dbContext) : IHomePageConfigRepository
{
    public Task<HomePageConfig?> GetDemoAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        return dbContext.HomepageConfigs
            .SingleOrDefaultAsync(
                x =>
                    x.Source == ContentSource.Demo &&
                    x.Created.UserId == userId,
                cancellationToken);
    }

    public Task<HomePageConfig?> GetOfficialAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.HomepageConfigs
            .SingleOrDefaultAsync(
                x => x.Source == ContentSource.Official,
                cancellationToken);
    }


    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

