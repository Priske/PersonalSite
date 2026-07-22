namespace PersonalSite.Api.Domain.Exceptions;

public class NotFoundException(
    string message) : Exception(message);
