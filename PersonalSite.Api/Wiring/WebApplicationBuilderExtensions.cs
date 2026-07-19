using BookTracker.Api.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;
using PersonalSite.Api.Storage;
using PersonalSite.Api.Storage.Users;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Security;
using Microsoft.IdentityModel.Tokens;
using PersonalSite.Api.Security.Password;
using PersonalSite.Api.Seeding;
using PersonalSite.Api.Infrastructure.Security.Password;

namespace PersonalSite.Api.Wiring;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        RegisterStorage(builder);
        RegisterHandlers(builder.Services);
        RegisterAuthentication(builder);

        return builder;
    }


    private static void RegisterStorage(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("PersonalSite")));

        builder.Services.AddScoped<IUserRepository, EfUserRepository>();
        builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        builder.Services.AddScoped<ICompromisedPasswordChecker, DatabaseCompromisedPasswordChecker>();
        builder.Services.AddScoped<IPasswordPolicy, PassphrasePasswordPolicy>();
        builder.Services.AddScoped<ISeedPasswordProvider, PassphraseSeedPasswordProvider>();
        builder.Services.AddScoped<UserFuzzr>();
    }

    private static void RegisterHandlers(IServiceCollection services)
    {
        var handlerTypes = HandlerMarker.Assembly
            .GetTypes()
            .Where(IsHandler);

        foreach (var type in handlerTypes)
        {
            services.AddScoped(type);
        }
    }

    private static bool IsHandler(Type type)
    {
        return type is { IsClass: true, IsAbstract: false }
            && type.IsAssignableTo(HandlerMarker);
    }

    private static void RegisterAuthentication(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration
            .GetRequiredSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are missing.");

        if (string.IsNullOrWhiteSpace(settings.SigningKey))
        {
            throw new InvalidOperationException("JWT signing key is missing.");
        }

        builder.Services.AddSingleton(settings);
        builder.Services.AddScoped<JwtTokenGenerator>();

        builder.Services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = settings.Issuer,

                        ValidateAudience = true,
                        ValidAudience = settings.Audience,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(settings.SigningKey)),

                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role,
                        ClockSkew = TimeSpan.Zero
                    };

            });
        builder.Services.AddAuthorization();

    }

    private static readonly Type HandlerMarker = typeof(IHandler);
}