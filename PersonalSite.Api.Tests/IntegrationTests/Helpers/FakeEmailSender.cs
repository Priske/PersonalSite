using System.Collections.Concurrent;
using PersonalSite.Api.Application.Mails;

namespace PersonalSite.Api.Tests.IntegrationTests.Helpers;

public sealed class FakeEmailSender : IEmailSender
{
    private readonly ConcurrentQueue<OutgoingEmail> sentEmails = new();

    public IReadOnlyCollection<OutgoingEmail> SentEmails =>
        sentEmails.ToArray();

    public Task SendAsync(
        OutgoingEmail email,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        sentEmails.Enqueue(email);

        return Task.CompletedTask;
    }
}