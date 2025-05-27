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
    IPasswordHasher<User> passwordHasher) : ICommandHandler<SignUpCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.FirstOrDefaultAsync(m => m.NormalizedEmail == command.Email.ToUpper(), cancellationToken: cancellationToken);
        if (exists.xIsNotEmpty())
        {
            switch (exists.EmailConfirmed)
            {
                case true:
                    return await Result<Guid>.FailureAsync("Email already exists.");
                case false:
                    context.DomainEventPublisher.Enqueue(new EmailVerifyUserDomainEvent(exists));
                    return await Result<Guid>.SuccessAsync(exists.Id);
            }
        }
        
        var user = User.Create(context, command.Email, command.UserName, command.PhoneNumber, ENUM_USER_TYPE.CUSTOMER);
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await Result<Guid>.SuccessAsync(user.Id);
    }
}