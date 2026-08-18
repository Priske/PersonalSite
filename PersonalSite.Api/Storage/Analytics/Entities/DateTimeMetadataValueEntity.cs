namespace PersonalSite.Api.Storage.Analytics.Entities;

internal sealed class DateTimeMetadataValueEntity
{
    public int Id { get; set; }

    public required DateTimeOffset Value { get; set; }
}


