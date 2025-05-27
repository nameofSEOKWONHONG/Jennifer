using eXtensionSharp;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

internal sealed class SignUpCommandHandler(
    ISessionContext context,
    JenniferDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    ISender sender) : ICommandHandler<SignUpCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.FirstAsync(m => m.NormalizedEmail == command.Email.ToUpper(), cancellationToken: cancellationToken);
        if (exists.xIsNotEmpty())
        {
            switch (exists.EmailConfirmed)
            {
                case true:
                    return Result<Guid>.Failure("Email already exists.");
                case false:
                    context.DomainEventPublisher.Enqueue(new EmailVerifyUserDomainEvent(exists));
                    return Result<Guid>.Success(exists.Id);
            }
        }
        
        var user = User.Create(context, command.Email, command.UserName, command.PhoneNumber, ENUM_USER_TYPE.CUSTOMER);
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}