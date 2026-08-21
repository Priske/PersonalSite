namespace PersonalSite.Api.Domain.Exceptions.contentSource
;

public class InvalidSourceException(
    string message) : OutofRangeException(message);
