namespace PersonalSite.Api.Domain.Exceptions;

public class NotFoundException(
    string message) : DomainException(message);
