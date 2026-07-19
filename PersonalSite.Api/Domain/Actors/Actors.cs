using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Domain.Actors;

public record Actor(
    int UserId,
    UserRole Role)
{
    public bool IsAdministrator =>
        Role == UserRole.Administrator;
}
