using MailKit.Security;
using MimeKit;
using PersonalSite.Api.Application.Mails;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace PersonalSite.Api.Infrastructure.Mails;

public sealed class SmtpEmailSender(
    SmtpSettings settings) : IEmailSender
{
    public async Task SendAsync(
        OutgoingEmail email,
        CancellationToken cancellationToken)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(
            email.FromName,
            settings.FromAddress));

        message.To.Add(
            MailboxAddress.Parse(email.ToAddress));

        if (!string.IsNullOrWhiteSpace(email.ReplyToAddress))
        {
            message.ReplyTo.Add(new MailboxAddress(
                email.ReplyToName ?? string.Empty,
                email.ReplyToAddress));
        }

        message.Subject = email.Subject;

        message.Body = new TextPart("plain")
        {
            Text = email.Body
        };

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(
            settings.Host,
            settings.Port,
            SecureSocketOptions.SslOnConnect,
            cancellationToken);

        await smtpClient.AuthenticateAsync(
            settings.Username,
            settings.Password,
            cancellationToken);

        await smtpClient.SendAsync(
            message,
            cancellationToken);

        await smtpClient.DisconnectAsync(
            quit: true,
            cancellationToken);
    }
}