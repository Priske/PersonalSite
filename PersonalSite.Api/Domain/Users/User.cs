namespace PersonalSite.Api.Domain.Users;

public class User
{
    public int Id { get; set; }

    public required UserName Name { get; set; }

    public required UserEmail Email { get; set; }

    public string PasswordHash { get; set; } = string.Empty;
    //public UserRole Role { get; set; } = UserRole.Member;
}
