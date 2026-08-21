namespace PersonalSite.Api.Application.Mails;

public interface IEmailSender
{
    Task SendAsync(
        OutgoingEmail email,
        CancellationToken cancellationToken);
}