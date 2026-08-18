using Microsoft.EntityFrameworkCore;
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

    public async Task<IReadOnlyList<Activity>> GetAsync(
        ActivityType type,
        DateTimeOffset? from,
        DateTimeOffset? to,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Activities
            .AsNoTracking()
            .Where(activity =>
                activity.Type == type);

        if (from is not null)
        {
            query = query.Where(activity =>
                activity.CreatedAt >= from);
        }

        if (to is not null)
        {
            query = query.Where(activity =>
                activity.CreatedAt <= to);
        }

        var activities = await query
            .Include(activity => activity.Metadata)
            .ToListAsync(cancellationToken);

        foreach (var activity in activities)
        {
            foreach (var metadata in activity.Metadata)
            {
                await LoadMetadataValues(
                    metadata,
                    cancellationToken);
            }
        }

        return activities;
    }

    private async Task LoadMetadataValues(
        ActivityMetadata metadata,
        CancellationToken cancellationToken)
    {
        var entries = await dbContext
            .Set<ActivityMetadataEntry>()
            .AsNoTracking()
            .Where(entry =>
                entry.ActivityMetadataId == metadata.Id)
            .ToListAsync(cancellationToken);

        foreach (var entry in entries)
        {
            var value = await LoadValue(
                entry.ValueType,
                entry.ValueId,
                cancellationToken);

            metadata.Add(
                entry.Key,
                value);
        }
    }

    private async Task<IMetadataValue> LoadValue(
        string valueType,
        int valueId,
        CancellationToken cancellationToken)
    {
        switch (valueType)
        {
            case nameof(StringMetadataValue):
                {
                    var entity = await dbContext
                        .Set<StringMetadataValueEntity>()
                        .AsNoTracking()
                        .SingleAsync(
                            x => x.Id == valueId,
                            cancellationToken);

                    return new StringMetadataValue(
                        entity.Value);
                }

            case nameof(ObjectMetadataValue):
                {
                    var objectValue = new ObjectMetadataValue();

                    var entries = await dbContext
                        .Set<ObjectMetadataValueEntry>()
                        .AsNoTracking()
                        .Where(entry =>
                            entry.ObjectMetadataValueId == valueId)
                        .ToListAsync(cancellationToken);

                    foreach (var entry in entries)
                    {
                        var childValue = await LoadValue(
                            entry.ValueType,
                            entry.ValueId,
                            cancellationToken);

                        objectValue.Add(
                            entry.Key,
                            childValue);
                    }

                    return objectValue;
                }

            default:
                throw new NotSupportedException(
                    $"Metadata value type '{valueType}' is not supported.");
        }
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