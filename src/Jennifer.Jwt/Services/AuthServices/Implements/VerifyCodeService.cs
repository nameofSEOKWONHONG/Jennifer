using eXtensionSharp;
using Jennifer.Jwt.Abstractions;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class VerifyCodeService: ServiceBase<VerifyCodeService, VerifyCodeRequest, VerifyCodeResponse>, IVerifyCodeService
{
    private readonly JenniferDbContext _applicationDbContext;

    public VerifyCodeService(ILogger<VerifyCodeService> logger,
        JenniferDbContext applicationDbContext) : base(logger)
    {
        _applicationDbContext = applicationDbContext;
    }

    public async Task<VerifyCodeResponse> HandleAsync(VerifyCodeRequest request, CancellationToken cancellationToken)
    {
        var emailVerify = await _applicationDbContext.EmailVerificationCodes
            .Where(m => m.Email == request.Email
                        && m.Type == request.Type
                        && !m.IsUsed 
                        && m.ExpiresAt > DateTimeOffset.UtcNow)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (emailVerify.xIsEmpty()) return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.NOT_FOUND, "인증 코드가 존재하지 않습니다.");
        if (emailVerify.FailedCount >= 5) return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.FAILED_COUNT_LIMIT, "인증 시도 횟수를 초과했습니다. 새 코드를 요청하세요.");
        if (emailVerify.Code != request.Code)
        {
            emailVerify.FailedCount++;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.WRONG_CODE, "잘못된 인증 코드입니다.");
        }
        
        emailVerify.IsUsed = true;
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return new VerifyCodeResponse(ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM, string.Empty);
    }
}