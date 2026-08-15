using System.Text.Json;

namespace PersonalSite.Api.Application.Analytics;

public sealed record ActivityMetadataRequest(
    string Key,
    JsonElement Value);