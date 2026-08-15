using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Storage.Analytics.Entities;

namespace PersonalSite.Api.Storage.Analytics;

public sealed class EFActivityRepository(AppDbContext dbContext)
    : IActivityRepository
{
    public async Task AddAsync(
        Activity activity,
        CancellationToken cancellationToken)
    {
        dbContext.Activities.Add(activity);

        await dbContext.SaveChangesAsync(cancellationToken);

        foreach (var metadata in activity.Metadata)
        {
            foreach (var (key, value) in metadata.Values)
            {
                var savedValue = await SaveValue(
                    value,
                    cancellationToken);

                var entry = new ActivityMetadataEntry
                {
                    ActivityMetadataId = metadata.Id,
                    Key = key,
                    ValueType = savedValue.ValueType,
                    ValueId = savedValue.ValueId
                };

                dbContext.Set<ActivityMetadataEntry>()
                    .Add(entry);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<(string ValueType, int ValueId)> SaveValue(
        IMetadataValue value,
        CancellationToken cancellationToken)
    {
        switch (value)
        {
            case StringMetadataValue stringValue:
                {
                    var entity = new StringMetadataValueEntity
                    {
                        Value = stringValue.Value
                    };

                    dbContext.Set<StringMetadataValueEntity>()
                        .Add(entity);

                    await dbContext.SaveChangesAsync(cancellationToken);

                    return (
                        nameof(StringMetadataValue),
                        entity.Id);
                }

            case ObjectMetadataValue objectValue:
                {
                    var entity = new ObjectMetadataValueEntity();

                    dbContext.Set<ObjectMetadataValueEntity>()
                        .Add(entity);

                    await dbContext.SaveChangesAsync(cancellationToken);

                    foreach (var (key, childValue) in objectValue.Values)
                    {
                        var savedChild = await SaveValue(
                            childValue,
                            cancellationToken);

                        var entry = new ObjectMetadataValueEntry
                        {
                            ObjectMetadataValueId = entity.Id,
                            Key = key,
                            ValueType = savedChild.ValueType,
                            ValueId = savedChild.ValueId
                        };

                        dbContext.Set<ObjectMetadataValueEntry>()
                            .Add(entry);
                    }

                    await dbContext.SaveChangesAsync(cancellationToken);

                    return (
                        nameof(ObjectMetadataValue),
                        entity.Id);
                }

            default:
                throw new NotSupportedException(
                    $"Metadata value type '{value.GetType().Name}' is not supported.");
        }
    }
}