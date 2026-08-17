namespace PersonalSite.Api.Analytics.Metadata;

public sealed class ActivityMetadata
{
    private readonly Dictionary<string, IMetadataValue> _values = [];
    public int Id { get; private set; }

    public IReadOnlyDictionary<string, IMetadataValue> Values => _values;

    public void Add(string key, IMetadataValue value)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new ArgumentException(
                "Metadata key is required.",
                nameof(key));
        }

        ArgumentNullException.ThrowIfNull(value);

        if (!_values.TryAdd(key, value))
        {
            throw new ArgumentException(
                $"Metadata key '{key}' already exists.",
                nameof(key));
        }

    }
}
