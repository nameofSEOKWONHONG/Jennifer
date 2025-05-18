using Jennifer.Jwt.Abstractions.Messaging;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Jennifer.Jwt.Application.Auth.Commands.SignUp;

public class SignUpCommandHandler(
    UserManager<User> userManager): ICommandHandler<SignUpCommand, IResult>
{
    public async Task<Result<IResult>> HandleAsync(SignUpCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Email = command.Email,
            UserName = command.UserName,
            EmailConfirmed = false,
            PhoneNumber = command.PhoneNumber,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            Type = ENUM_USER_TYPE.CUSTOMER,
            CreatedOn = DateTimeOffset.UtcNow
        };
        var result = await userManager.CreateAsync(user, command.Password);
        if (!result.Succeeded)
        {
            // 충돌 에러 존재하면 409, 그 외는 400
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                return TypedResults.Conflict(new
                {
                    Message = "이미 등록된 사용자입니다.",
                    Errors = result.Errors
                });
            }

            return TypedResults.BadRequest(new
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