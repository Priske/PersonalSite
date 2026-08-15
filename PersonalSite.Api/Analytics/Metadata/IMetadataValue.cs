namespace PersonalSite.Api.Analytics.Metadata;

public interface IMetadataValue
{
}

public sealed record StringMetadataValue(string Value) : IMetadataValue;

public sealed record IntegerMetadataValue(int Value) : IMetadataValue;

public sealed record DecimalMetadataValue(decimal Value) : IMetadataValue;

public sealed record BooleanMetadataValue(bool Value) : IMetadataValue;

public sealed record DateTimeMetadataValue(DateTimeOffset Value) : IMetadataValue;