
namespace PersonalSite.Api.Security.Password;

public interface IPasswordPolicy
{
    Task ValidateAsync(
        string password,
        CancellationToken cancellationToken = default);
}