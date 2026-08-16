namespace PersonalSite.Api.Analytics.Metadata;

public interface IMetadataValue
{
}

public sealed record StringMetadataValue(string Value) : IMetadataValue;

public sealed record IntegerMetadataValue(int Value) : IMetadataValue;

public sealed record DecimalMetadataValue(decimal Value) : IMetadataValue;

public sealed record BooleanMetadataValue(bool Value) : IMetadataValue;

public sealed record DateTimeMetadataValue(DateTimeOffset Value) : IMetadataValue;

public sealed class ObjectMetadataValue : IMetadataValue
{
    private readonly Dictionary<string, IMetadataValue> _values = [];

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