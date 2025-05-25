using Jennifer.Account.Behaviors;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Models.Contracts;
using Jennifer.Account.Session.Abstracts;
using Jennifer.Infrastructure.Abstractions.Behaviors;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

internal sealed class SignUpCommandHandler(
    ISessionContext context,
    JenniferDbContext dbContext,
    IPasswordHasher<User> passwordHasher,
    IDomainEventPublisher domainEventPublisher) : ICommandHandler<SignUpCommand, Result<Guid>>
{
    public async ValueTask<Result<Guid>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        var exists = dbContext.Users.Any(m => m.NormalizedEmail == command.Email.ToUpper());
        if (exists)
        {
            return Result<Guid>.Failure("email already exists.");
        }
        
        var user = User.Create(context, command.Email, command.UserName, command.PhoneNumber, ENUM_USER_TYPE.CUSTOMER);
        user.PasswordHash = passwordHasher.HashPassword(user, command.Password);
        
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(user.Id);
    }
}