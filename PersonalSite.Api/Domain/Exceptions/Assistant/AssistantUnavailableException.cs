namespace PersonalSite.Api.Domain.Exceptions.Assistant;

public sealed class AssistantUnavailableException(
    string message,
    Exception innerException)
    : Exception(message, innerException);