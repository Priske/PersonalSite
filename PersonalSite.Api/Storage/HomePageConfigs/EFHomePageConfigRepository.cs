using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.HomePageConfigs;

public class EfHomePageRepository(AppDbContext dbContext) : IHomePageConfigRepository
{
    public async Task<HomePageConfig?> GetDemoAsync(int userId,
    CancellationToken cancellationToken = default)
    {
        return await dbContext.HomepageConfigs
            .SingleOrDefaultAsync(
                x =>
                    x.Source == ContentSource.Demo &&
                    x.CreatedByUserId == userId,
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

