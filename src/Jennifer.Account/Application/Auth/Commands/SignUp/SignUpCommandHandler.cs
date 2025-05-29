using eXtensionSharp;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Domain.Account;
using Jennifer.Domain.Account.Contracts;
using Jennifer.Domain.Database;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

internal sealed class SignUpCommandHandler(
    ISessionContext session,
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
                    session.DomainEventPublisher.Enqueue(new UserCompleteDomainEvent(exists));
                    return await Result<Guid>.SuccessAsync(exists.Id);
            }
        }
        
        var user = User.Create(session.DomainEventPublisher, command.Email, command.UserName, command.PhoneNumber, ENUM_USER_TYPE.CUSTOMER);
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await Result<Guid>.SuccessAsync(user.Id);
    }
}