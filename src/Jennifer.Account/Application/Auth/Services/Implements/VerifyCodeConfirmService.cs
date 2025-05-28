using eXtensionSharp;
using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Account.Application.Auth.Services.Abstracts;
using Jennifer.Account.Data;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Account.Application.Auth.Services.Implements;

internal sealed class VerifyCodeConfirmService(JenniferDbContext dbContext): 
    ServiceBase<VerifyCodeRequest, Result>, IVerifyCodeConfirmService
{
    protected override async Task<Result> HandleAsync(VerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var verified = await dbContext.EmailVerificationCodes
            .Where(m => m.Email == request.Email
                        && m.Type == request.Type
                        && !m.IsUsed 
                        && m.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (verified.xIsEmpty()) return await Result.FailureAsync("인증 코드가 존재하지 않습니다.");
        if (verified.FailedCount >= 5) return await Result.FailureAsync("인증 시도 횟수를 초과했습니다. 새 코드를 요청하세요.");
        if (verified.Code != request.Code)
        {
            verified.FailedCount++;
            await dbContext.SaveChangesAsync(cancellationToken);
            return await Result.FailureAsync("잘못된 인증 코드입니다.");
        }
        
        verified.IsUsed = true;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await Result.SuccessAsync();
    }
}