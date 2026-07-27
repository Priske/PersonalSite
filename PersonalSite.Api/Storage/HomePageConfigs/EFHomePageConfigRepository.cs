using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Domain.HomePageConfigs;

namespace PersonalSite.Api.Storage.HomePageConfigs;

public class EfHomePageRepository(AppDbContext dbContext) : IHomePageConfigRepository
{

    public Task<HomePageConfig?> GetAsync(
           CancellationToken cancellationToken)
    {
        return dbContext.HomepageConfigs
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

