namespace PersonalSite.Api.Domain.Exceptions.user;

public class InvalidUserRoleException(
    string message) : DomainException(message);