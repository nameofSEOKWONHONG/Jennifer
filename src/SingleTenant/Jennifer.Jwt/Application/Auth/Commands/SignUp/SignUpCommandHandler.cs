using Jennifer.Infrastructure.Abstractions.Messaging;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public class SignUpCommandHandler(
    JenniferDbContext dbContext,
    IPasswordHasher<User> passwordHasher): ICommandHandler<SignUpCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        var exists = dbContext.Users.Any(m => m.NormalizedEmail == command.Email.ToUpper());
        if (exists)
        {
            return Result.Failure<Guid>(Error.NotFound(string.Empty, "Email already exists"));
        }
        
        var user = new User
        {
            Email = command.Email,
            NormalizedEmail = command.Email.ToUpper(),
            UserName = command.UserName,
            NormalizedUserName = command.UserName.ToUpper(),
            EmailConfirmed = false,
            PhoneNumber = command.PhoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            Type = ENUM_USER_TYPE.CUSTOMER,
        };
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        await dbContext.Users.AddAsync(user, cancellationToken);
        
        user.Raise(new SignUpDomainEvent(user));
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}