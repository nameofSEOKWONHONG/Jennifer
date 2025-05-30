using Confluent.Kafka;
using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Domain.Accounts;
using Jennifer.Domain.Accounts.Contracts;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.Infrastructure.Database;
using Jennifer.Infrastructure.Email;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Services.Implements;

public sealed class VerifyCodeSendEmailService(
    JenniferDbContext dbContext,
    IProducer<string, string> producer) : ServiceBase<VerifyCodeSendEmailRequest, Result>, 
    IVerifyCodeSendEmailService {
    
    protected override async Task<Result> HandleAsync(VerifyCodeSendEmailRequest request, CancellationToken cancellationToken)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var emailSubject = "Jennifer 이메일 인증 코드 안내";
        var emailFormat = @"안녕하세요. 

{0}님,
Jennifer 서비스 이용을 위한 이메일 인증 코드를 안내해 드립니다.
아래의 인증 코드를 입력창에 입력해 주세요.

인증 코드: {1}

※ 인증 코드는 발급 시점으로부터 30분간 유효합니다.
※ 인증 시도 5회 실패 시 새로운 인증 코드를 발급받으셔야 합니다.
※ 본인이 요청하지 않은 인증 코드라면 이 메일을 무시해 주세요.

본 메일은 발신전용이며 회신되지 않습니다.

감사합니다.
Jennifer";

        var templateSubject = await dbContext.Options.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Type == ENUM_OPTION_TYPE.WELCOME_MESSAGE_SUBJECT, cancellationToken: cancellationToken);
        var templateFormat = await dbContext.Options.AsNoTracking()
            .FirstOrDefaultAsync(m => m.Type == ENUM_OPTION_TYPE.WELCOME_MESSAGE_BODY, cancellationToken: cancellationToken);
        if (templateSubject.xIsNotEmpty()) emailSubject = templateSubject.Value;
        if (templateFormat.xIsNotEmpty()) emailFormat = templateFormat.Value;

        await dbContext.EmailVerificationCodes.AddAsync(new EmailConfirmCode()
        {
            Email = request.Email,
            Type = request.Type,
            Code = code,
            FailedCount = 0,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30),
            CreatedAt = DateTimeOffset.UtcNow,
            IsUsed = false
        }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var emailBody = string.Format(emailFormat, request.UserName.xValue<string>(request.Email), code);
        var mail = new EmailMessageBuilder()
            .To(request.UserName.xIsEmpty().xValue<string>(request.Email), request.Email)
            .Subject(emailSubject)
            .Body(emailBody)
            .IsHtml(false)
            .Build();
        
        var message = new Message<string, string>();
        message.Key = Guid.NewGuid().ToString();
        message.Value = mail.xSerialize();
        await producer.ProduceAsync("email", message, cancellationToken);

        return await Result.SuccessAsync();
    }
}