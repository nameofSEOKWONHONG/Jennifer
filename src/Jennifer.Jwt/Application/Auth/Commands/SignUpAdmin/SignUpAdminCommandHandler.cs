using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUpAdmin;

internal class SignUpAdminCommandHandler(JenniferDbContext context, IPasswordHasher<User> passwordHasher)
    : ICommandHandler<SignUpAdminCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(SignUpAdminCommand command, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(m => m.Email == command.Email, cancellationToken: cancellationToken))
        {
            return Result.Failure<Guid>(UserErrors.EmailNotUnique);
        }

        var user = new User()
        {
            Id = Guid.CreateVersion7(),
            Email = command.Email,
            UserName = command.Email,
            EmailConfirmed = true,
            PhoneNumber = null,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            SecurityStamp = Guid.NewGuid().ToString()         
        };
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        
        user.Raise(new SignUpAdminDomainEvent(user.Id));
        
        context.Users.Add(user);
        
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success(user.Id);
    }
}