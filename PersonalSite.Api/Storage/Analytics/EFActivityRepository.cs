
using PersonalSite.Api.Analytics;

namespace PersonalSite.Api.Storage.Analytics;

public class EFActivityRepository(AppDbContext dbContext) : IActivityRepository
{
    public async Task AddAsync(Activity activity, CancellationToken cancellationToken)
    {
        dbContext.Activities.Add(activity);

        await dbContext.SaveChangesAsync(cancellationToken);


    }
}
