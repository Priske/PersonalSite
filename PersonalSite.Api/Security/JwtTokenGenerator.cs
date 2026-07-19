using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.IdentityModel.Tokens;
using PersonalSite.Api.Application.Auth.Login;
using PersonalSite.Api.Domain.Users;

namespace PersonalSite.Api.Security;

public class JwtTokenGenerator(JwtSettings settings)
{
    public LoginResponse Generate(User user)
    {
        var expiresAt =
            DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes);

        var claims =
            new List<Claim>
            {
                 new(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),
                new(
                    ClaimTypes.Role,
                    user.Role.ToString())
            };

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(settings.SigningKey));

        var credentials =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer: settings.Issuer,
                audience: settings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

        var value =
            new JwtSecurityTokenHandler().WriteToken(token);

        return
            new LoginResponse
            {
                AccessToken = value,
                ExpiresAt = expiresAt
            };
    }
}