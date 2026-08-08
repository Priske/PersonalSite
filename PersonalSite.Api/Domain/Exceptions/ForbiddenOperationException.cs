namespace PersonalSite.Api.Domain.Exceptions;

public class ForbiddenOperationException(
    string message) : DomainException(message);
