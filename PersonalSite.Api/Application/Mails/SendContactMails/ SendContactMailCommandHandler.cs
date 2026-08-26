using PersonalSite.Api.Domain.Mails;

namespace PersonalSite.Api.Application.Mails.SendContactMails;

public sealed class SendContactMailCommandHandler(
    IEmailSender emailSender) : IHandler
{
    public async Task Execute(
        SendContactMailRequest request,
        CancellationToken cancellationToken)
    {
        var name = new ContactName(request.Name);
        var email = new ContactEmail(request.Email);
        var message = new ContactMessage(request.Message);

        var outgoingEmail = new OutgoingEmail(
            FromName: "Contact Form",
            ToAddress: "contact@beneeckman.be",
            Subject: $"[Portfolio contact] Message from {name.Value}",
            Body: $"""
                Name: {name.Value}
                Email: {email.Value}

                {message.Value}
                """,
            ReplyToAddress: email.Value,
            ReplyToName: name.Value);

        await emailSender.SendAsync(
            outgoingEmail,
            cancellationToken);
    }
}