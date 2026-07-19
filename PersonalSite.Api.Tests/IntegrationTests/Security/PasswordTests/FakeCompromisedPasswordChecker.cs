using PersonalSite.Api.Security.Password;

namespace PersonalSite.Api.Tests.IntegrationTests.Security.PasswordTests;

internal sealed class FakeCompromisedPasswordChecker
    : ICompromisedPasswordChecker
{
    private readonly HashSet<string> _compromisedPasswords;

    public bool WasCalled { get; private set; }

    public string? LastCheckedPassword { get; private set; }

    public FakeCompromisedPasswordChecker(
        IEnumerable<string>? compromisedPasswords = null)
    {
        _compromisedPasswords = new HashSet<string>(
            compromisedPasswords ?? [],
            StringComparer.Ordinal);
    }

    public Task<bool> IsCompromisedAsync(
        string password,
        CancellationToken cancellationToken = default)
    {
        WasCalled = true;
        LastCheckedPassword = password;

        return Task.FromResult(
            _compromisedPasswords.Contains(password));
    }
}