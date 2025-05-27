using eXtensionSharp;
using FluentValidation;
using Jennifer.Account.Data;
using Jennifer.Account.Models;
using Jennifer.Account.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

internal sealed record ModifyUserRequest(Guid UserId, string UserName, string PhoneNumber);
internal sealed record ModifyUserCommand(Guid UserId, string UserName, string PhoneNumber): ICommand<Result>;

internal sealed class ModifyUserCommandHandler(
    JenniferDbContext dbContext): ICommandHandler<ModifyUserCommand, Result>
{
    public async ValueTask<Result> Handle(ModifyUserCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == command.UserId, cancellationToken: cancellationToken);
        if(exists.xIsEmpty()) return await Result.FailureAsync("not found user");
        
        exists.PhoneNumber = command.PhoneNumber;
        exists.UserName = command.UserName;
        exists.NormalizedUserName = command.UserName.ToUpper();
        exists.ConcurrencyStamp = Guid.NewGuid().ToString();
        
        await dbContext.SaveChangesAsync(cancellationToken);

        return await Result.SuccessAsync();
    }
}

internal sealed class ModifyUserCommandValidator : AbstractValidator<ModifyUserCommand>
{
    public ModifyUserCommandValidator()
    {
        RuleFor(m => m.PhoneNumber).NotNull();
        RuleFor(m => m.UserName).NotNull();       
        RuleFor(m => m.UserId).NotEmpty();       
    }
}

internal sealed record AddOrUpdateUserRoleCommand(Guid UserId, Guid RoleId): ICommand<Result>;
internal sealed class AddOrUpdateUserRoleCommandHandler(JenniferDbContext dbContext): ICommandHandler<AddOrUpdateUserRoleCommand, Result>
{
    public async ValueTask<Result> Handle(AddOrUpdateUserRoleCommand command, CancellationToken cancellationToken)
    {
        var exists = await dbContext.UserRoles.FirstOrDefaultAsync(m => m.UserId == command.UserId, cancellationToken: cancellationToken);
        if (exists.xIsEmpty())
        {
            var role = new UserRole()
            {
                UserId = command.UserId,
                RoleId = command.RoleId
            };
            await dbContext.UserRoles.AddAsync(role, cancellationToken);
        }
        else
        {
            exists.RoleId = command.RoleId;
            dbContext.UserRoles.Update(exists);
        }
        
        await dbContext.SaveChangesAsync(cancellationToken);
        
        return await Result.SuccessAsync();
    }
}

internal sealed class AddOrUpdateUserRoleCommandValidator : AbstractValidator<AddOrUpdateUserRoleCommand>
{
    public AddOrUpdateUserRoleCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
        RuleFor(m => m.RoleId).NotEmpty();       
    }
}