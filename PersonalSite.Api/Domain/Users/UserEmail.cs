using PersonalSite.Api.Domain.Common;


namespace PersonalSite.Api.Domain.Users;

public sealed record UserEmail
{
    public string Value { get; }

    public UserEmail(string? value)
    {
        Value = EmailValue.Create(value, "User email");
    }


    public static implicit operator string(UserEmail email)
    => email.Value;

    public override string ToString()
        => Value;
}

