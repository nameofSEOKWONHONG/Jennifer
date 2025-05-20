using eXtensionSharp;
using Jennifer.Infrastructure.Abstractions;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Application.Auth.Services.Abstracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Application.Auth.Services.Implements;

public class VerifyCodeConfirmService: ServiceBase<VerifyCodeConfirmService, VerifyCodeRequest, VerifyCodeResponse>, IVerifyCodeConfirmService
{
    private readonly JenniferDbContext _dbContext;

    public VerifyCodeConfirmService(ILogger<VerifyCodeConfirmService> logger,
        JenniferDbContext dbContext) : base(logger)
    {
        _dbContext = dbContext;
    }

    public async Task<VerifyCodeResponse> HandleAsync(VerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var verified = await _dbContext.EmailVerificationCodes
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
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.WRONG_CODE, "잘못된 인증 코드입니다.");
        }
        
        verified.IsUsed = true;
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM, string.Empty);
    }
}