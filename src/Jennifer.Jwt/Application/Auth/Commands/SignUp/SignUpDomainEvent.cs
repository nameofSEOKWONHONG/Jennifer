using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Infrastructure.Email;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public sealed record SignUpDomainEvent(User User): IDomainEvent;

public class SignUpDomainEventHandler(ILogger<SignUpDomainEventHandler> logger,
    JenniferDbContext dbContext,
    IEmailQueue emailQueue): IDomainEventHandler<SignUpDomainEvent>
{
    public async Task Handle(SignUpDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogDebug("Domain Event:{UserId}", domainEvent.User.Id);
        
        var code = new Random().Next(100000, 999999).ToString();
        var emailSubject = "Jennifer 이메일 인증 안내";
        var emailFormat = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
</head>
<body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
        <p>안녕하세요,</p>
        <p>Jennifer 서비스 이용을 위한 이메일 인증 안내해 드립니다.</p>
        <p>아래의 인증 버튼을 클릭해 주세요.</p>
        
        <div style='text-align: center; margin: 30px 0;'>
            <a href='https://localhost:5000/verify-email?code={0}&email={1}' 
               style='background-color: #007bff; 
                      color: white; 
                      padding: 12px 24px; 
                      text-decoration: none; 
                      border-radius: 4px;
                      display: inline-block;'>
                이메일 인증하기
            </a>
        </div>

        <p style='color: #666; font-size: 0.9em;'>
            ※ 인증 링크는 발급 시점으로부터 30분간 유효합니다.<br>
            ※ 인증 시도 5회 실패 시 새로운 인증 코드를 발급받으셔야 합니다.<br>
            ※ 본인이 요청하지 않은 인증이라면 이 메일을 무시해 주세요.
        </p>

        <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>

        <p style='color: #999; font-size: 0.8em;'>
            본 메일은 발신전용이며 회신되지 않습니다.<br>
            감사합니다.<br>
            Jennifer
        </p>
    </div>
</body>
</html>";

        // EmailMessage 생성 시 IsHtml 플래그를 true로 설정
        var emailBody = string.Format(emailFormat, code, domainEvent.User.Email);
        var mail = new EmailMessage.Builder()
            .From("Jennifer", "<EMAIL>")
            .To(domainEvent.User.UserName, domainEvent.User.Email)
            .Subject(emailSubject)
            .Body(emailBody)
            .IsHtml(true) // HTML 이메일임을 명시
            .Build();
        
        await dbContext.EmailVerificationCodes.AddAsync(new EmailVerificationCode()
        {
            Email = domainEvent.User.Email,
            Type = ENUM_EMAIL_VERIFICATION_TYPE.SIGN_UP_BEFORE,
            Code = code,
            FailedCount = 0,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30),
            CreatedAt = DateTimeOffset.UtcNow,
            IsUsed = false
        }, cancellationToken);
        
        await emailQueue.EnqueueAsync(mail, cancellationToken);
    }
}