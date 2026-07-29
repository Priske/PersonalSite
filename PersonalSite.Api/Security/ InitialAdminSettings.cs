namespace PersonalSite.Api.Security;

public class InitialAdminSettings
{
    public const string SectionName = "InitialAdmin";

    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string Password { get; set; }
}