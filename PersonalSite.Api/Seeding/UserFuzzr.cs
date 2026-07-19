using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Security.Password;
using QuickFuzzr;

namespace PersonalSite.Api.Seeding;

public sealed class UserFuzzr(
    ISeedPasswordProvider passwordProvider,
    IPasswordPolicy passwordPolicy,
    IPasswordHasher<User> passwordHasher)
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

    public async Task<IReadOnlyList<User>> ManyAsync(
        int count,
        CancellationToken cancellationToken = default)
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

            await passwordPolicy.ValidateAsync(
                seed.Password,
                cancellationToken);

            var user = new User
            {
                Name = new UserName(
                    $"{seed.FirstName} {seed.LastName}"),

                Email = new UserEmail(
                    $"{seed.FirstName}.{seed.LastName}.{index}@example.test"
                        .ToLowerInvariant())
            };

            user.PasswordHash = passwordHasher.HashPassword(
                user,
                seed.Password);

            users.Add(user);
        }

        return users;
    }

    private FuzzrOf<UserSeedData> CreateFuzzr() =>
        from firstName in Fuzzr.OneOf(FirstNames)
        from lastName in Fuzzr.OneOf(LastNames)
        from password in passwordProvider.Password
        select new UserSeedData(
            firstName,
            lastName,
            password);

    private sealed record UserSeedData(
        string FirstName,
        string LastName,
        string Password);
}