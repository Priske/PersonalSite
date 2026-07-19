using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonalSite.Api.Security.Password;
using PersonalSite.Api.Storage;

namespace PersonalSite.Api.Infrastructure.Security.Password;

public sealed class DatabaseCompromisedPasswordChecker(
    AppDbContext dbContext)
    : ICompromisedPasswordChecker
{
    public async Task<bool> IsCompromisedAsync(
        string password,
        CancellationToken cancellationToken = default)
    {
        var hash = ComputeSha1(password);

        return await dbContext.CompromisedPasswordHashes
            .AnyAsync(
                item => item.Hash == hash,
                cancellationToken);
    }

    private static string ComputeSha1(string password)
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = SHA1.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}