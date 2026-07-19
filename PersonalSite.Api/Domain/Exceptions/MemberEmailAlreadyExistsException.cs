namespace PersonalSite.Api.Domain.Exceptions;

public class UserEmailAlreadyExistsException()
    : DomainException("A member with this email already exists.");