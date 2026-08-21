namespace PersonalSite.Api.Application.Mails;

public sealed record OutgoingEmail(
    string FromName,
    string ToAddress,
    string Subject,
    string Body,
    string? ReplyToAddress = null,
    string? ReplyToName = null);