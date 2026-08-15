using System.Text.Json.Serialization;

namespace PersonalSite.Api.Analytics;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ActivityType
{
    PageViewed,
    User_Registered,
    Login,
    Logout,
    DemoHomepageUpdated,
    DemoProjectCreated,
    DemoProjectUpdated,
    DemoProjectDeleted
}