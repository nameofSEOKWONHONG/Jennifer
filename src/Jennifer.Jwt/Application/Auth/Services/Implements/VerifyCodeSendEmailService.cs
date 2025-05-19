using eXtensionSharp;
using Jennifer.Jwt.Abstractions;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Infrastructure.Email;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel.Infrastructure.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Services.Implements;

public class VerifyCodeSendEmailService : ServiceBase<VerifyCodeSendEmailService, VerifyCodeSendEmailRequest, IResult>, 
    IVerifyCodeSendEmailService {
    private readonly JenniferDbContext _applicationDbContext;
    private readonly IEmailQueue _emailQueue;

    public VerifyCodeSendEmailService(
        ILogger<VerifyCodeSendEmailService> logger,
        JenniferDbContext applicationDbContext,
        IEmailQueue emailQueue) : base(logger)
    {
        _applicationDbContext = applicationDbContext;
        _emailQueue = emailQueue;
    }

    public async Task<IResult> HandleAsync(VerifyCodeSendEmailRequest request, CancellationToken cancellationToken)
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

        await _applicationDbContext.EmailVerificationCodes.AddAsync(new EmailVerificationCode()
        {
            Email = request.Email,
            Type = request.Type,
            Code = code,
            FailedCount = 0,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30),
            CreatedAt = DateTimeOffset.UtcNow,
            IsUsed = false
        }, cancellationToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        var emailBody = string.Format(emailFormat, request.UserName.xValue<string>(request.Email), code);
        var mail = new EmailMessage.Builder()
            .To(request.UserName.xIsEmpty().xValue<string>(request.Email), request.Email)
            .Subject(emailSubject)
            .Body(emailBody)
            .IsHtml(false)
            .Build();
        
        await _emailQueue.EnqueueAsync(mail, cancellationToken);

        return TypedResults.Ok(code);
    }
}