using System.Net;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PersonalSite.Api.Application.Assistant;
using PersonalSite.Api.Storage;
using PersonalSite.Api.Storage.Files;
using PersonalSite.Api.Tests.IntegrationTests.Helpers;

namespace PersonalSite.Api.Tests.IntegrationTests.Application;

public sealed class AssistantKnowledgeTests
{
    private sealed class Store : IFileStorage
    {
        public ConcurrentDictionary<string, byte[]> Files { get; } = [];
        public bool FailUpload { get; set; }
        public bool FailDelete { get; set; }
        public async Task UploadAsync(string fileName, Stream content, string contentType, bool overwrite, CancellationToken ct)
        {
            if (FailUpload) throw new IOException("Fictional storage failure");
            Assert.False(overwrite); Assert.False(Files.ContainsKey(fileName));
            using var bytes = new MemoryStream(); await content.CopyToAsync(bytes, ct); Assert.True(Files.TryAdd(fileName, bytes.ToArray()));
        }
        public Task<Stream?> OpenReadAsync(string fileName, CancellationToken ct) =>
            Task.FromResult(Files.TryGetValue(fileName, out var bytes) ? (Stream?)new MemoryStream(bytes) : null);
        public Task DeleteAsync(string fileName, CancellationToken ct)
        { if (FailDelete) throw new IOException("Fictional cleanup failure"); Files.TryRemove(fileName, out _); return Task.CompletedTask; }
    }

    [Fact]
    public async Task Same_filename_replacement_preserves_unrelated_blobs_metadata_and_assistant_knowledge()
    {
        using var baseFactory = new CustomWebApplicationFactory(); var store = new Store();
        using var factory = baseFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        { services.RemoveAll<IFileStorage>(); services.AddSingleton<IFileStorage>(store); }));
        using var client = factory.CreateClient();
        // Use a genuine test login, never production credentials.
        await Login(client);
        using var capabilities = await client.GetAsync("/file/assistantknowledge");
        capabilities.EnsureSuccessStatusCode();
        var contract = await capabilities.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Assert.True(contract.GetProperty("replaceExisting").GetBoolean());
        Assert.False(contract.TryGetProperty("replaceAll", out _));
        (await Upload(client, "old-a.md", "OLD A")).EnsureSuccessStatusCode();
        (await Upload(client, "old-b.md", "OLD B")).EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.BadRequest, (await Upload(client, "old-a.md", "duplicate")).StatusCode);
        var unrelatedBlobs = store.Files.ToDictionary();
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var unrelatedMetadata = await db.StoredFiles.AsNoTracking().OrderBy(f => f.Id)
            .Select(f => new { f.Id, f.StorageKey, f.OriginalFileName, f.ContentType, f.SizeInBytes }).ToArrayAsync();
        (await Upload(client, "careerkit-profile-en.md", "NEW FIRST", true)).EnsureSuccessStatusCode();
        Assert.Equal(3, store.Files.Count);
        (await Upload(client, "CAREERKIT-PROFILE-EN.MD", "NEW SECOND", true)).EnsureSuccessStatusCode();
        Assert.Equal(3, await db.AssistantKnowledgeFiles.CountAsync());
        Assert.Equal(3, await db.StoredFiles.CountAsync()); Assert.Equal(3, store.Files.Count);
        foreach (var (key, bytes) in unrelatedBlobs) Assert.Equal(bytes, store.Files[key]);
        Assert.Equal(unrelatedMetadata, await db.StoredFiles.AsNoTracking().Where(f => f.OriginalFileName != "careerkit-profile-en.md").OrderBy(f => f.Id)
            .Select(f => new { f.Id, f.StorageKey, f.OriginalFileName, f.ContentType, f.SizeInBytes }).ToArrayAsync());
        var knowledge = await scope.ServiceProvider.GetRequiredService<AssistantKnowledgeReader>().ReadAsync(default);
        Assert.Contains("NEW SECOND", knowledge); Assert.DoesNotContain("NEW FIRST", knowledge);
        Assert.Contains("OLD A", knowledge); Assert.Contains("OLD B", knowledge);
        Assert.Equal(HttpStatusCode.BadRequest, (await Upload(client, "careerkit-profile-en.md", "duplicate")).StatusCode);
        (await Upload(client, "manual.md", "manual text")).EnsureSuccessStatusCode();
        Assert.Equal(4, store.Files.Count);
    }

    [Fact]
    public async Task Invalid_unauthorized_and_storage_failure_preserve_active_knowledge()
    {
        using var baseFactory = new CustomWebApplicationFactory(); var store = new Store();
        using var factory = baseFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        { services.RemoveAll<IFileStorage>(); services.AddSingleton<IFileStorage>(store); }));
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Unauthorized, (await Upload(client, "profile.md", "text", true)).StatusCode);
        await Login(client);
        (await Upload(client, "rules.md", "KEEP RULES")).EnsureSuccessStatusCode();
        (await Upload(client, "profile.md", "ORIGINAL")).EnsureSuccessStatusCode();
        var original = store.Files.ToDictionary();
        Assert.Equal(HttpStatusCode.BadRequest, (await Upload(client, "profile.md", " ", true)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Upload(client, "profile.txt", "text", true)).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await Upload(client, "profile.md", new string('x', 256 * 1024 + 1), true)).StatusCode);
        store.FailUpload = true;
        await Assert.ThrowsAsync<IOException>(() => Upload(client, "profile.md", "replacement", true));
        Assert.Equal(original.Count, store.Files.Count);
        foreach (var (key, bytes) in original) Assert.Equal(bytes, store.Files[key]);
        store.FailUpload = false; store.FailDelete = true;
        await Assert.ThrowsAsync<InvalidOperationException>(() => Upload(client, "profile.md", "ACTIVE NEW", true));
        using var scope = factory.Services.CreateScope();
        var knowledge = await scope.ServiceProvider.GetRequiredService<AssistantKnowledgeReader>().ReadAsync(default);
        Assert.Contains("ACTIVE NEW", knowledge); Assert.DoesNotContain("ORIGINAL", knowledge);
        Assert.Contains("KEEP RULES", knowledge);
        var rules = Assert.Single(original.Where(pair => pair.Key.EndsWith("rules.md")));
        Assert.Equal(rules.Value, store.Files[rules.Key]);
    }

    private static async Task Login(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/auth/login", new { Email = "admin@integration.test", Password = "Integration-Test-Password-123!" });
        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<PersonalSite.Api.Application.Auth.Login.LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new("Bearer", login!.AccessToken);
    }

    [Fact]
    public async Task Concurrent_replacements_serialize_and_database_failure_removes_only_uncommitted_new_blob()
    {
        using var baseFactory = new CustomWebApplicationFactory(); var store = new Store();
        using var factory = baseFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        { services.RemoveAll<IFileStorage>(); services.AddSingleton<IFileStorage>(store); }));
        using var client = factory.CreateClient(); await Login(client);
        (await Upload(client, "rules.md", "KEEP RULES")).EnsureSuccessStatusCode();
        var rules = Assert.Single(store.Files);
        var responses = await Task.WhenAll(Enumerable.Range(1, 4).Select(i => Upload(client, "profile.md", $"REPLACEMENT {i}", true)));
        foreach (var response in responses) { response.EnsureSuccessStatusCode(); response.Dispose(); }
        var original = store.Files.ToDictionary(); Assert.Equal(2, original.Count);
        Assert.Equal(rules.Value, store.Files[rules.Key]);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            Assert.Equal(2, await db.AssistantKnowledgeFiles.CountAsync()); Assert.Equal(2, await db.StoredFiles.CountAsync());
            await db.Database.ExecuteSqlRawAsync("""
                CREATE FUNCTION reject_test_file() RETURNS trigger LANGUAGE plpgsql AS $$ BEGIN RAISE EXCEPTION 'test write failure'; END $$;
                CREATE TRIGGER reject_test_file BEFORE INSERT ON "StoredFiles" FOR EACH ROW EXECUTE FUNCTION reject_test_file();
                """);
        }
        await Assert.ThrowsAsync<DbUpdateException>(() => Upload(client, "profile.md", "SHOULD ROLL BACK", true));
        Assert.Equal(original.Count, store.Files.Count);
        foreach (var (key, bytes) in original) Assert.Equal(bytes, store.Files[key]);
        using var fresh = factory.Services.CreateScope();
        var knowledge = await fresh.ServiceProvider.GetRequiredService<AssistantKnowledgeReader>().ReadAsync(default);
        Assert.Contains("REPLACEMENT", knowledge); Assert.Contains("KEEP RULES", knowledge); Assert.DoesNotContain("SHOULD ROLL BACK", knowledge);
    }
    private static async Task<HttpResponseMessage> Upload(HttpClient client, string name, string text, bool replace = false)
    {
        using var form = new MultipartFormDataContent();
        using var file = new ByteArrayContent(Encoding.UTF8.GetBytes(text)); file.Headers.ContentType = new("text/markdown");
        form.Add(file, "file", name);
        return await client.PostAsync("/file/assistantknowledge" + (replace ? "?replaceExisting=true" : ""), form);
    }

    [Theory]
    [InlineData("replaceAll=true")]
    [InlineData("replaceAll=false")]
    [InlineData("replaceAll=true&replaceExisting=true")]
    [InlineData("REPLACEALL=true")]
    public async Task Retired_replace_all_requests_are_rejected_without_modifying_any_files(string query)
    {
        using var baseFactory = new CustomWebApplicationFactory(); var store = new Store();
        using var factory = baseFactory.WithWebHostBuilder(builder => builder.ConfigureServices(services =>
        { services.RemoveAll<IFileStorage>(); services.AddSingleton<IFileStorage>(store); }));
        using var client = factory.CreateClient(); await Login(client);
        (await Upload(client, "rules.md", "KEEP RULES")).EnsureSuccessStatusCode();
        (await Upload(client, "profile.md", "KEEP PROFILE")).EnsureSuccessStatusCode();
        var original = store.Files.ToDictionary();
        using var form = new MultipartFormDataContent();
        using var file = new ByteArrayContent(Encoding.UTF8.GetBytes("MUST NOT BE SAVED")); file.Headers.ContentType = new("text/markdown");
        form.Add(file, "file", "profile.md");
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync("/file/assistantknowledge?" + query, form)).StatusCode);
        Assert.Equal(original.Count, store.Files.Count);
        foreach (var (key, bytes) in original) Assert.Equal(bytes, store.Files[key]);
        using var scope = factory.Services.CreateScope();
        Assert.Equal(2, await scope.ServiceProvider.GetRequiredService<AppDbContext>().StoredFiles.CountAsync());
        var knowledge = await scope.ServiceProvider.GetRequiredService<AssistantKnowledgeReader>().ReadAsync(default);
        Assert.Contains("KEEP RULES", knowledge); Assert.Contains("KEEP PROFILE", knowledge); Assert.DoesNotContain("MUST NOT BE SAVED", knowledge);
    }
}
