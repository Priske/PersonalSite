namespace PersonalSite.Api.Application.Users.GetUserSummeries;

public class UserSummary
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}