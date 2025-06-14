﻿using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;

namespace Jennifer.Account.Application.Auth.Commands.Password;

public sealed class PasswordForgotVerifyCommandHandler(
    IServiceExecutionBuilderFactory factory
):ICommandHandler<PasswordForgotVerifyCommand, Result>
{
    public async ValueTask<Result> Handle(PasswordForgotVerifyCommand command, CancellationToken cancellationToken)
    {
        Result result = null;
        var builder = factory.Create();
        await builder.Register<IEmailConfirmService, EmailConfirmRequest, Result>()
            .Request(new EmailConfirmRequest(command.Email, command.Code, ENUM_EMAIL_VERIFY_TYPE.PASSWORD_FORGOT.Name))
            .Handle(r => result = r)
            .ExecuteAsync(cancellationToken);
        
        if(!result.IsSuccess) return await Result.FailureAsync(result.Message);
        
        return await Result.SuccessAsync();
    }
}
