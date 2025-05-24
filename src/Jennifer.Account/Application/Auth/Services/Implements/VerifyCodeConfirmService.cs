using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Data;
using Jennifer.Account.Session;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Account.Application.Auth.Services.Implements;

internal sealed class VerifyCodeConfirmService(ILogger<VerifyCodeConfirmService> logger, JenniferDbContext dbContext): 
    ServiceBase<VerifyCodeRequest, VerifyCodeResponse>, IVerifyCodeConfirmService
{
    protected override async Task<VerifyCodeResponse> HandleAsync(VerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var verified = await dbContext.EmailVerificationCodes
            .Where(m => m.Email == request.Email
                        && m.Type == request.Type
                        && !m.IsUsed 
                        && m.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (verified.xIsEmpty()) return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.NOT_FOUND, "인증 코드가 존재하지 않습니다.");
        if (verified.FailedCount >= 5) return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.FAILED_COUNT_LIMIT, "인증 시도 횟수를 초과했습니다. 새 코드를 요청하세요.");
        if (verified.Code != request.Code)
        {
            verified.FailedCount++;
            await dbContext.SaveChangesAsync(cancellationToken);
            return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.WRONG_CODE, "잘못된 인증 코드입니다.");
        }
        
        verified.IsUsed = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM, string.Empty);
    }
}