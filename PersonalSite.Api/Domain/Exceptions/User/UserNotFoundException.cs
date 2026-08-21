namespace PersonalSite.Api.Domain.Exceptions.user;

public class UserNotFoundException(
    string message) : NotFoundException(message);