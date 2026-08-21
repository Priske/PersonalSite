namespace PersonalSite.Api.Infrastructure.Mails;

public sealed class SmtpSettings
{
    public const string SectionName = "Smtp";

    public required string Host { get; init; }
    public int Port { get; init; } = 465;
    public required string Username { get; init; }
    public required string Password { get; init; }

    public required string FromAddress { get; init; }
}