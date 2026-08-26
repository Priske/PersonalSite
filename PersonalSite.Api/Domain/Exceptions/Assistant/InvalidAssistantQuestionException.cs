using PersonalSite.Api.Domain.Exceptions;
namespace PersonalSite.Api.Domain.Exceptions.Assistant;

public sealed class InvalidAssistantQuestionException(string message) : DomainException(message);