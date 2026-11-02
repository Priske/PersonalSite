using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Application.Analytics.GetContactLinkActivity;
using PersonalSite.Api.Application.Auth.Login;
using PersonalSite.Api.Application.Redirects;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Domain.Redirects;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Storage;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application.Analytics;

public sealed class RedirectLinkTests : IDisposable
{
    private readonly CustomWebApplicationFactory factory = new();
    private readonly HttpClient client;

    public RedirectLinkTests() => client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public void Dispose() { client.Dispose(); factory.Dispose(); }

    private async Task Authenticate(UserRole role)
    {
        var user = new User
        {
            Name = new UserName("Redirect Test"), Email = new UserEmail("redirect@example.test"),
            PasswordHash = string.Empty, Role = role
        };
        user.PasswordHash = new PasswordHasher<User>().HashPassword(user, "redirect-test-long-password");
        factory.GetWriter().Seed(db => db.Users.Add(user));
        var response = await client.PostAsJsonAsync("/auth/login", new LoginRequest
        {
            Email = "redirect@example.test", Password = "redirect-test-long-password"
        });
        response.EnsureSuccessStatusCode();
        var login = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login.AccessToken);
    }

    private Task<HttpResponseMessage> Save(string slug = "cv-en", string target = "https://example.test/cv", string? campaign = "global", bool active = true) =>
        client.PutAsJsonAsync($"/redirect-links/{slug}", new SaveRedirectLinkRequest(target, campaign, active));

    [Theory]
    [InlineData(null, HttpStatusCode.Unauthorized)]
    [InlineData(UserRole.User, HttpStatusCode.Forbidden)]
    public async Task Management_and_analytics_require_administrator(UserRole? role, HttpStatusCode expected)
    {
        if (role is not null) await Authenticate(role.Value);
        Assert.Equal(expected, (await client.GetAsync("/redirect-links")).StatusCode);
        Assert.Equal(expected, (await Save()).StatusCode);
        Assert.Equal(expected, (await client.GetAsync("/analytics/redirect-links")).StatusCode);
        Assert.Empty(factory.GetReader().Query(db => db.RedirectLinks.ToList()));
    }

    [Fact]
    public async Task Anonymous_visit_uses_saved_target_records_sanitized_metadata_and_does_not_mix_contact_counts()
    {
        await Authenticate(UserRole.Administrator);
        (await Save()).EnsureSuccessStatusCode();
        client.DefaultRequestHeaders.Authorization = null;
        using var request = new HttpRequestMessage(HttpMethod.Get, "/r/cv-en?target=https://attacker.test&campaign=forged");
        request.Headers.Referrer = new Uri("https://name:password@referrer.test/private/path?secret=value#fragment");
        using var visit = await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Redirect, visit.StatusCode);
        Assert.Equal("https://example.test/cv", visit.Headers.Location!.AbsoluteUri);
        Assert.True(visit.Headers.CacheControl!.NoStore);
        Assert.Equal("no-referrer", Assert.Single(visit.Headers.GetValues("Referrer-Policy")));

        using var fresh = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        // A separate anonymous client has no referrer/session. No request follows the external target.
        Assert.Equal(HttpStatusCode.Redirect, (await fresh.GetAsync("/r/cv-en")).StatusCode);

        var persisted = factory.GetReader().Query(db => db.Activities.Include(a => a.Metadata)
            .Where(a => a.Type == ActivityType.LinkClicked).ToList());
        Assert.Equal(2, persisted.Count);
        Assert.All(persisted, activity => Assert.Null(activity.UserId));

        // Restore genuine test-session auth without adding another user.
        var login = await client.PostAsJsonAsync("/auth/login", new LoginRequest
        {
            Email = "redirect@example.test", Password = "redirect-test-long-password"
        });
        client.DefaultRequestHeaders.Authorization = new("Bearer", (await login.Content.ReadFromJsonAsync<LoginResponse>())!.AccessToken);
        var analytics = (await client.GetFromJsonAsync<RedirectAnalyticsResponse>("/analytics/redirect-links"))!;
        Assert.Equal(2, analytics.TotalVisits);
        Assert.Contains(analytics.Links, item => item is { Slug: "cv-en", Campaign: "global", Referrer: "https://referrer.test", Visits: 1 });
        Assert.Contains(analytics.Links, item => item.Referrer == "Direct / unavailable");
        Assert.DoesNotContain("secret", System.Text.Json.JsonSerializer.Serialize(analytics));
        var contacts = (await client.GetFromJsonAsync<ContactLinkAnalyticsResponse>("/analytics/contact-links"))!;
        Assert.Equal(0, contacts.TotalClicks);
    }

    [Fact]
    public async Task Updating_destination_preserves_slug_creation_date_and_old_visit_campaigns_and_can_disable()
    {
        await Authenticate(UserRole.Administrator);
        var first = (await (await Save()).Content.ReadFromJsonAsync<RedirectLinkResponse>())!;
        Assert.Equal(HttpStatusCode.Redirect, (await client.GetAsync("/r/cv-en")).StatusCode);
        var updated = (await (await Save(target: "https://example.test/new", campaign: "updated")).Content.ReadFromJsonAsync<RedirectLinkResponse>())!;
        Assert.Equal(first.CreatedAt, updated.CreatedAt);
        Assert.Equal(first.Slug, updated.Slug);
        Assert.Equal("https://example.test/new", (await client.GetAsync("/r/cv-en")).Headers.Location!.AbsoluteUri);
        Assert.Single((await client.GetFromJsonAsync<RedirectLinkResponse[]>("/redirect-links"))!);
        var analytics = (await client.GetFromJsonAsync<RedirectAnalyticsResponse>("/analytics/redirect-links"))!;
        Assert.Equal(new[] { "global", "updated" }, analytics.Links.Select(item => item.Campaign).Order().ToArray());

        (await Save(active: false)).EnsureSuccessStatusCode();
        var inactive = await client.GetAsync("/r/cv-en");
        Assert.Equal(HttpStatusCode.NotFound, inactive.StatusCode);
        Assert.True(inactive.Headers.CacheControl!.NoStore);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/r/unknown")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync("/r/CV-en")).StatusCode);
        Assert.Equal(2, (await client.GetFromJsonAsync<RedirectAnalyticsResponse>("/analytics/redirect-links"))!.TotalVisits);
    }

    [Theory]
    [InlineData("CV-en", "https://example.test", null)]
    [InlineData("cv--en", "https://example.test", null)]
    [InlineData("cv_en", "https://example.test", null)]
    [InlineData("cv-en", "javascript:alert(1)", null)]
    [InlineData("cv-en", "/relative", null)]
    [InlineData("cv-en", "//example.test", null)]
    [InlineData("cv-en", "https://user:password@example.test", null)]
    [InlineData("cv-en", "https://example.test/\r\nLocation:bad", null)]
    [InlineData("cv-en", "https://example.test\\bad", null)]
    [InlineData("cv-en", "https://example.test", "line\nbreak")]
    [InlineData("cv-en", null, null)]
    public async Task Invalid_saves_do_not_mutate_existing_destination(string slug, string? target, string? campaign)
    {
        await Authenticate(UserRole.Administrator);
        (await Save()).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, (await Save(slug, target!, campaign)).StatusCode);
        var saved = Assert.Single((await client.GetFromJsonAsync<RedirectLinkResponse[]>("/redirect-links"))!);
        Assert.Equal("https://example.test/cv", saved.TargetUrl);
    }

    [Fact]
    public async Task Concurrent_creation_has_one_link_and_analytics_filters_do_not_include_contact_events()
    {
        await Authenticate(UserRole.Administrator);
        var responses = await Task.WhenAll(Enumerable.Range(0, 4).Select(_ => Save("application-2026-01", campaign: "application")));
        foreach (var response in responses) { response.EnsureSuccessStatusCode(); response.Dispose(); }
        Assert.Single((await client.GetFromJsonAsync<RedirectLinkResponse[]>("/redirect-links"))!);
        (await Save()).EnsureSuccessStatusCode();
        await client.GetAsync("/r/cv-en");
        await client.GetAsync("/r/application-2026-01");
        var filtered = (await client.GetFromJsonAsync<RedirectAnalyticsResponse>("/analytics/redirect-links?search=application&sortBy=slug&descending=false"))!;
        Assert.Equal("application-2026-01", Assert.Single(filtered.Links).Slug);
        Assert.Equal(1, filtered.TotalVisits);
        Assert.Empty((await client.GetFromJsonAsync<RedirectAnalyticsResponse>("/analytics/redirect-links?from=2099-01-01T00:00:00Z"))!.Links);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync("/analytics/redirect-links?from=2030-01-01T00:00:00Z&to=2020-01-01T00:00:00Z")).StatusCode);
    }

    [Fact]
    public async Task Metadata_failure_rolls_back_partial_activity_but_redirect_still_works()
    {
        await Authenticate(UserRole.Administrator);
        (await Save()).EnsureSuccessStatusCode();
        factory.GetWriter().Seed(db => db.Database.ExecuteSqlRaw("""
            CREATE FUNCTION reject_redirect_metadata() RETURNS trigger AS $$
            BEGIN RAISE EXCEPTION 'test metadata failure'; END; $$ LANGUAGE plpgsql;
            CREATE TRIGGER reject_redirect_metadata BEFORE INSERT ON "ActivityMetadataEntry"
            FOR EACH ROW EXECUTE FUNCTION reject_redirect_metadata();
            """));
        var response = await client.GetAsync("/r/cv-en");
        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Empty(factory.GetReader().Query(db => db.Activities.Where(a => a.Type == ActivityType.LinkClicked).ToList()));
    }

    [Fact]
    public async Task Value_objects_round_trip_through_existing_string_columns_and_query_by_value()
    {
        var slug = new RedirectSlug("cv-en");
        var target = new RedirectTargetUrl("https://example.test/path%2Fkept?q=a%20b#section");
        factory.GetWriter().Seed(db => db.RedirectLinks.Add(new RedirectLink
        {
            Slug = slug, TargetUrl = target, Campaign = new RedirectCampaign(" english-cv "),
            IsActive = true, CreatedAt = DateTimeOffset.UtcNow
        }));

        // New scopes force EF to materialize values from PostgreSQL, not a tracked entity.
        var restored = factory.GetReader().Query(db => db.RedirectLinks.AsNoTracking()
            .Single(link => link.Slug == new RedirectSlug("cv-en")));
        Assert.Equal(slug, restored.Slug);
        Assert.Equal(target, restored.TargetUrl);
        Assert.Equal(new RedirectCampaign("english-cv"), restored.Campaign);
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entity = context.Model.FindEntityType(typeof(RedirectLink))!;
        foreach (var name in new[] { nameof(RedirectLink.Slug), nameof(RedirectLink.TargetUrl), nameof(RedirectLink.Campaign) })
            Assert.Equal(typeof(string), entity.FindProperty(name)!.GetTypeMapping().Converter!.ProviderClrType);
        Assert.Equal("character varying(80)", entity.FindProperty(nameof(RedirectLink.Slug))!.GetColumnType());
        Assert.Equal("character varying(2048)", entity.FindProperty(nameof(RedirectLink.TargetUrl))!.GetColumnType());
        Assert.Equal("character varying(200)", entity.FindProperty(nameof(RedirectLink.Campaign))!.GetColumnType());
        Assert.Equal(target.Value, (await client.GetAsync("/r/cv-en")).Headers.Location!.OriginalString);

        await Authenticate(UserRole.Administrator);
        var response = Assert.Single((await client.GetFromJsonAsync<RedirectLinkResponse[]>("/redirect-links"))!);
        Assert.Equal(slug.Value, response.Slug);
        Assert.Equal(target.Value, response.TargetUrl);
        Assert.Equal("english-cv", response.Campaign);
        (await Save(target: target.Value, campaign: "   ")).EnsureSuccessStatusCode();
        Assert.Null(factory.GetReader().Query(db => db.RedirectLinks.AsNoTracking().Single()).Campaign);
        Assert.Null(Assert.Single((await client.GetFromJsonAsync<RedirectLinkResponse[]>("/redirect-links"))!).Campaign);
    }

    [Fact]
    public async Task Direct_handler_calls_cannot_bypass_domain_validation_or_write_partial_data()
    {
        await Authenticate(UserRole.Administrator);
        (await Save()).EnsureSuccessStatusCode();
        using var scope = factory.Services.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<RedirectLinkHandler>();
        await Assert.ThrowsAsync<DomainException>(() => handler.SaveAsync("CV", new("https://example.test", null, true), default));
        await Assert.ThrowsAsync<DomainException>(() => handler.SaveAsync("cv-en", new("javascript:alert(1)", null, true), default));
        await Assert.ThrowsAsync<DomainException>(() => handler.SaveAsync("cv-en", new("https://example.test", "invalid\ncampaign", true), default));
        var saved = factory.GetReader().Query(db => db.RedirectLinks.AsNoTracking().Single());
        Assert.Equal("https://example.test/cv", saved.TargetUrl.Value);
        Assert.Equal("global", saved.Campaign!.Value);
    }
}
