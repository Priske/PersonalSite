namespace PersonalSite.Api.Security.Password;

public interface ICompromisedPasswordChecker
{
    Task<bool> IsCompromisedAsync(
        string password,
        CancellationToken cancellationToken = default);
}