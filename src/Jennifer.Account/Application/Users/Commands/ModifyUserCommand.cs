using System.Data;
using eXtensionSharp;
using FluentValidation;
using Jennifer.Domain.Accounts;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Extenstions;
using Jennifer.Infrastructure.Session;
using Jennifer.Infrastructure.Session.Abstracts;
using Jennifer.SharedKernel;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Users.Commands;

public sealed record ModifyUserRequest(Guid UserId, string UserName, string PhoneNumber);

[UseTransaction(IsolationLevel.ReadUncommitted)]
public sealed record ModifyUserCommand(Guid UserId, string UserName, string PhoneNumber): ICommand<Result>;


public sealed class ModifyUserCommandValidator : AbstractValidator<ModifyUserCommand>
{
    public ModifyUserCommandValidator()
    {
        RuleFor(m => m.PhoneNumber).NotNull();
        RuleFor(m => m.UserName).NotNull();       
        RuleFor(m => m.UserId).NotEmpty();       
    }
}

public sealed class AddOrUpdateUserRoleCommandValidator : AbstractValidator<AddOrUpdateUserRoleCommand>
{
    public AddOrUpdateUserRoleCommandValidator()
    {
        RuleFor(m => m.UserId).NotEmpty();
        RuleFor(m => m.RoleId).NotEmpty();       
    }
}