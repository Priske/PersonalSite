namespace PersonalSite.Api.Storage.Analytics.Entities;

internal sealed class ActivityMetadataEntry
{
    public int Id { get; set; }

    public int ActivityMetadataId { get; set; }

    public required string Key { get; set; }

    public required string ValueType { get; set; }

    public int ValueId { get; set; }
}
