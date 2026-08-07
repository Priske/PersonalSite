namespace PersonalSite.Api.Application.Users.UpdateUsers;

public class UpdateUserRequest
{
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string Role { get; set; }

}
