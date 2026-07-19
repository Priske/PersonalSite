using BookTracker.Api.Application;
using Microsoft.AspNetCore.Identity;
using PersonalSite.Api.Domain.Exceptions;
using PersonalSite.Api.Domain.Users;
using PersonalSite.Api.Storage.Users;

namespace PersonalSite.Api.Application.Users.CreateUsers;

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher<User> passwordHasher) : IHandler
{

    public async Task<CreateUserResponse> Execute(CreateUserRequest request)
    {

        var mail = new UserEmail(request.Email);
        if (await userRepository.EmailExistsAsync(mail))
        {
            throw new UserEmailAlreadyExistsException();
        }

        var user =
            new User
            {
                Name = new UserName(request.Name),
                Email = mail,
                //Role = MemberRole.Member
            };

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new DomainException("Password is required.");
        }

        if (request.Password.Length < 8)
        {
            throw new DomainException("Password must contain at least 8 characters.");
        }
        user.PasswordHash = passwordHasher.HashPassword(user, request.Password);

        var savedUser = await userRepository.AddAsync(user);
        return
            new CreateUserResponse
            {
                Id = savedUser.Id,
                Name = savedUser.Name,
                Email = savedUser.Email,

            };
    }

}
