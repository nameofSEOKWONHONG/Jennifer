using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class SignUpService: ServiceBase<SignUpService, RegisterRequest, IResult>, ISignUpService
{
    private readonly UserManager<User> _userManager;
    private readonly IVerifyCodeService _verifyCodeService;


    public SignUpService(ILogger<SignUpService> logger,
        UserManager<User> userManager,
        IVerifyCodeService verifyCodeService) : base(logger)
    {
        _userManager = userManager;
        _verifyCodeService = verifyCodeService;
    }

    public async Task<IResult> HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var verified = await _verifyCodeService.HandleAsync(new VerifyCodeRequest(request.Email, request.VerifyCode, request.Type), cancellationToken);
        if (verified.Status != ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM)
            return TypedResults.BadRequest(verified);
        
        var user = new User
        {
            Email = request.Email,
            UserName = request.UserName,
            EmailConfirmed = true,
            PhoneNumber = request.PhoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            Type = ENUM_USER_TYPE.CUSTOMER,
            CreatedOn = DateTimeOffset.UtcNow
        };
        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            // 충돌 에러 존재하면 409, 그 외는 400
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                return Results.Conflict(new
                {
                    Message = "이미 등록된 사용자입니다.",
                    Errors = result.Errors
                });
            }

            return Results.BadRequest(new
            {
                Message = "회원가입 실패",
                Errors = result.Errors.Select(e => new
                {
                    e.Code,
                    e.Description
                })
            });
        }

        return TypedResults.Ok(user.Id.ToString());
    }
}

