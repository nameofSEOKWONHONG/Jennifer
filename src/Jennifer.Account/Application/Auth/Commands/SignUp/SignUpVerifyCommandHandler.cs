using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.Infrastructure.Database;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Commands.SignUp;

public sealed class SignUpVerifyCommandHandler(
    JenniferDbContext dbContext,
    IServiceExecutionBuilderFactory factory): ICommandHandler<SignUpVerifyCommand, Result>
{
    public async ValueTask<Result> Handle(SignUpVerifyCommand command, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstAsync(m => m.Id == command.UserId, cancellationToken: cancellationToken);

        Result verified = null;
        var builder = factory.Create();
        await builder.Register<IEmailConfirmService, EmailConfirmRequest, Result>()
            .Request(new EmailConfirmRequest(user.Email, command.Code, ENUM_EMAIL_VERIFY_TYPE.SIGN_UP_BEFORE.Name))
            .Handle(r => verified = r)
            .ExecuteAsync(cancellationToken);
                
        if(!verified.IsSuccess) 
            return await Result.FailureAsync(verified.Message);

        user.Confirm();

        await dbContext.SaveChangesAsync(cancellationToken);

        return await Result.SuccessAsync();
    }
}
