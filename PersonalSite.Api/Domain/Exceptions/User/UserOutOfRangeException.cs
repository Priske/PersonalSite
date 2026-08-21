namespace PersonalSite.Api.Domain.Exceptions.user;

public class UserOutOfRangeException(
    string message) : OutofRangeException(message);