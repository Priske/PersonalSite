namespace PersonalSite.Api.Domain.Exceptions;

public class TagInUseException(
    string message) : DomainException(message);
