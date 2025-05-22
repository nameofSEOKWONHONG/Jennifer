using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public class SignUpCommandHandler(
    JenniferDbContext dbContext,
    IPasswordHasher<User> passwordHasher) : ICommandHandler<SignUpCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        var exists = dbContext.Users.Any(m => m.NormalizedEmail == command.Email.ToUpper());
        if (exists)
        {
            return Result<Guid>.Failure("email already exists.");
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
        
        user.Raise(new SignUpNotification(user));
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}