namespace PersonalSite.Api.Application.Mails.SendContactMails.cs;

public sealed record SendContactMailRequest
{
    public required string Name { get; init; }
    public required string Email { get; init; }
    public required string Message { get; init; }
}