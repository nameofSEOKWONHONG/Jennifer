using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed class SignUpAdminCommandHandler(JenniferDbContext context, IPasswordHasher<User> passwordHasher)
    : ICommandHandler<SignUpAdminCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(SignUpAdminCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(m => m.Email == command.Email, cancellationToken: cancellationToken))
        {
            return await Result<Guid>.FailureAsync("email already exists.");
        }

        var user = new User()
        {
            Id = Guid.CreateVersion7(),
            Email = command.Email,
            NormalizedEmail = command.Email.ToUpper(),
            UserName = command.UserName,
            NormalizedUserName = command.UserName.ToUpper(),
            EmailConfirmed = true,
            PhoneNumber = command.PhoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString(),
        };
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        
        context.Users.Add(user);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return await Result<Guid>.SuccessAsync(user.Id);
    }
}