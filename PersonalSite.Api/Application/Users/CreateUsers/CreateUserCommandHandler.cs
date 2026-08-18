using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Analytics;
using PersonalSite.Api.Analytics.Metadata;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.CreateUsers;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher,
    ActivityTracker activityTracker) : IHandler
{
    public async Task<CreateUserResponse> Execute(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var mail = new UserEmail(request.Email);

        if (await userRepository.EmailExistsAsync(
                mail,
                cancellationToken))
        {
            throw new UserEmailAlreadyExistsException();
        }

        var user = new User
        {
            Name = new UserName(request.Name),
            Email = mail,
        };

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new DomainException(
                "Password is required.");
        }

        if (request.Password.Length < 8)
        {
            throw new DomainException(
                "Password must contain at least 8 characters.");
        }

        user.PasswordHash =
            passwordHasher.HashPassword(
                user,
                request.Password);

        var savedUser =
            await userRepository.AddAsync(
                user,
                cancellationToken);

        await activityTracker.TrackAsync(
            ActivityType.CreatedUser,
            savedUser.Id,
            metadata =>
            {
                var createdUser =
                    new ObjectMetadataValue();

                createdUser.Add(
                    "Name",
                    new StringMetadataValue(
                        savedUser.Name.ToString()));

                createdUser.Add(
                    "Email",
                    new StringMetadataValue(
                        savedUser.Email.ToString()));

                metadata.Add(
                    "CreatedUser",
                    createdUser);
            },
            cancellationToken);

        return new CreateUserResponse
        {
            Id = savedUser.Id,
            Name = savedUser.Name,
            Email = savedUser.Email,
        };
    }
}