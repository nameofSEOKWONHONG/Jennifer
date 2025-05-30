using eXtensionSharp;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed class SignUpCommandHandler(
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