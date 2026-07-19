namespace PersonalSite.Api.Application.Users.GetUserDetails;

public class GetUserDetailsResponse
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }

    public required string Role { get; init; }

}
