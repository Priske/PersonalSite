using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using PersonalSite.Api.Storage;
using Testcontainers.PostgreSql;

namespace PersonalSite.Api.Tests.IntegrationTests.Helpers;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly PostgreSqlContainer PostgresContainer = new PostgreSqlBuilder("postgres:17")
        .WithDatabase("postgres")
        .WithUsername("personal_site_tests")
        .WithPassword("test-password")
        .Build();

    private static readonly Lazy<Task> ContainerStartup = new(() => PostgresContainer.StartAsync());

    private readonly string databaseName = $"personal_site_test_{Guid.NewGuid():N}";
    private readonly string connectionString;

    private static readonly KeyValuePair<string, string?>[] TestSettings =
    [
        new("SeedDatabase", "false"),

    new("Jwt:Issuer", "PersonalSite.Tests"),
    new("Jwt:Audience", "PersonalSite.Tests"),
    new("Jwt:SigningKey", "personal-site-test-signing-key-with-32-characters"),
    new("Jwt:ExpirationMinutes", "10"),

    new("InitialAdmin:Name", "Integration Test Admin"),
    new("InitialAdmin:Email", "admin@integration.test"),
    new("InitialAdmin:Password", "Integration-Test-Password-123!")
    ];
    public CustomWebApplicationFactory()
    {
        ContainerStartup.Value.GetAwaiter().GetResult();

        connectionString = new NpgsqlConnectionStringBuilder(PostgresContainer.GetConnectionString())
        {
            Database = databaseName
        }.ConnectionString;

        CreateDatabase();
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureHostConfiguration(configuration =>
            configuration.AddInMemoryCollection(TestSettings));

        return base.CreateHost(builder);
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
        });

        builder.ConfigureAppConfiguration((context, configuration) =>
            configuration.AddInMemoryCollection(TestSettings));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.RemoveAll<AppDbContext>();

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        });
    }

    private void CreateDatabase()
    {
        var adminConnectionString = new NpgsqlConnectionStringBuilder(PostgresContainer.GetConnectionString())
        {
            Database = "postgres"
        }.ConnectionString;

        using var connection = new NpgsqlConnection(adminConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = $"""CREATE DATABASE "{databaseName}";""";
        command.ExecuteNonQuery();
    }

    private void DropDatabase()
    {
        var adminConnectionString = new NpgsqlConnectionStringBuilder(PostgresContainer.GetConnectionString())
        {
            Database = "postgres"
        }.ConnectionString;

        using var connection = new NpgsqlConnection(adminConnectionString);
        connection.Open();

        using var terminateCommand = connection.CreateCommand();
        terminateCommand.CommandText =
            """
            SELECT pg_terminate_backend(pid)
            FROM pg_stat_activity
            WHERE datname = @databaseName
              AND pid <> pg_backend_pid();
            """;
        terminateCommand.Parameters.AddWithValue("databaseName", databaseName);
        terminateCommand.ExecuteNonQuery();

        using var dropCommand = connection.CreateCommand();
        dropCommand.CommandText = $"""DROP DATABASE IF EXISTS "{databaseName}";""";
        dropCommand.ExecuteNonQuery();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            base.Dispose(disposing);
            return;
        }

        base.Dispose(disposing);
        NpgsqlConnection.ClearAllPools();
        DropDatabase();
    }

    public EfReader GetReader() => new(Services);

    public EfWriter GetWriter() => new(Services);
}