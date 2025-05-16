using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Base;
using Jennifer.SharedKernel.Infrastructure.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class VerifyCodeByEmailSendService : ServiceBase<VerifyCodeByEmailSendService, VerifyCodeByEmailSendRequest, IResult>, IVerifyCodeByEmailSendService
{
    private readonly JenniferDbContext _dbContext;
    private readonly IEmailQueue _emailQueue;

    public VerifyCodeByEmailSendService(
        ILogger<VerifyCodeByEmailSendService> logger,
        JenniferDbContext dbContext,
        IEmailQueue emailQueue) : base(logger)
    {
        _dbContext = dbContext;
        _emailQueue = emailQueue;
    }

    public async Task<IResult> HandleAsync(VerifyCodeByEmailSendRequest request, CancellationToken cancellationToken)
    {
        var code = new Random().Next(100000, 999999).ToString();
        var emailSubject = "Jennifer 이메일 인증 코드 안내";
        var emailFormat = @"안녕하세요,

Jennifer 서비스 이용을 위한 이메일 인증 코드를 안내해 드립니다.
아래의 인증 코드를 입력창에 입력해 주세요.

인증 코드: {0}

※ 인증 코드는 발급 시점으로부터 30분간 유효합니다.
※ 인증 시도 5회 실패 시 새로운 인증 코드를 발급받으셔야 합니다.
※ 본인이 요청하지 않은 인증 코드라면 이 메일을 무시해 주세요.

본 메일은 발신전용이며 회신되지 않습니다.

감사합니다.
Jennifer";

        await _dbContext.EmailVerificationCodes.AddAsync(new EmailVerificationCode()
        {
            Email = request.Email,
            Type = request.Type,
            Code = code,
            FailedCount = 0,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30),
            CreatedAt = DateTimeOffset.UtcNow,
            IsUsed = false
        }, cancellationToken);

        var emailBody = string.Format(emailFormat, code);
        var mail = new EmailMessage.Builder()
            .From("Jennifer", "<EMAIL>")
            .To(request.Email, request.Email)
            .Subject(emailSubject)
            .Body(emailBody)
            .Build();
        
        await _emailQueue.EnqueueAsync(mail, cancellationToken);

        return TypedResults.Ok(code);
    }
}