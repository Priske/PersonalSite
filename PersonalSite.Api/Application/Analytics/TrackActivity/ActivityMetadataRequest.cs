using System.Text.Json;

namespace PersonalSite.Api.Application.Analytics.TrackActivity;

public sealed record ActivityMetadataRequest(
    string Key,
    JsonElement Value);