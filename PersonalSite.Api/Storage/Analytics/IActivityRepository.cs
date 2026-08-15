
using PersonalSite.Api.Analytics;

namespace PersonalSite.Api.Storage.Analytics;

public interface IActivityRepository
{

    Task AddAsync(Activity activity, CancellationToken cancellationToken);


}
