namespace PersonalSite.Api.Domain.Exceptions;

public class OutofRangeException(
    string message) : DomainException(message);
