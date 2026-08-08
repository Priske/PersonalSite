using PersonalSite.Api.Domain.Users;
using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public sealed class UserFuzzr()
{
    private static readonly string[] FirstNames =
    [
        "Ada",
        "Grace",
        "Douglas",
        "Ursula",
        "Terry",
        "Octavia",
        "Isaac",
        "Mary",
        "Kurt",
        "Agatha"
    ];

    private static readonly string[] LastNames =
    [
        "Byte",
        "Stackwell",
        "Nullman",
        "Loopington",
        "Brackets",
        "Mergefield",
        "Bugworthy",
        "Semicolon",
        "Heap",
        "Async"
    ];
    private static readonly string[] Mails =
    [
        "@Gmail.com",
        "@Hotmail.com",
        "@Yahoo.com",
        "@telenet.be",
        "@proximus.be",
        "@outlook.com",
        "@icloud.com",
        "@aol.com",
        "@proton.me",
        "@live.com"
    ];

    public async Task<IReadOnlyList<User>> ManyAsync(
        int count)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(count);

        var seeds = CreateFuzzr()
            .Many(count)
            .Generate()
            .ToList();

        var users = new List<User>(seeds.Count);

        for (var index = 0; index < seeds.Count; index++)
        {
            var seed = seeds[index];
            var user = new User
            {
                Name = new UserName(
                    $"{seed.FirstName} {seed.LastName}"),

                Email = new UserEmail(
                    $"{seed.FirstName}.{seed.LastName}.{index}{seed.Mail}"
                        .ToLowerInvariant()),
                Role = UserRole.FakeUser
            };
            users.Add(user);
        }
        return users;
    }

    private FuzzrOf<UserSeedData> CreateFuzzr() =>
        from firstName in Fuzzr.OneOf(FirstNames)
        from lastName in Fuzzr.OneOf(LastNames)
        from mail in Fuzzr.OneOf(Mails)
        select new UserSeedData(
            firstName,
            lastName,
            mail
            );

    private sealed record UserSeedData(
        string FirstName,
        string LastName,
        string Mail
        );
}