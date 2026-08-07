using System.ComponentModel.DataAnnotations.Schema;

namespace PersonalSite.Api.Domain;

[ComplexType]
public record Change(int? UserId, DateTimeOffset At);