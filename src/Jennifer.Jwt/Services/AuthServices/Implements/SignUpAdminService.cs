using eXtensionSharp;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Models.Contracts;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.SharedKernel.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class SignUpAdminService : ServiceBase<SignUpAdminService, RegisterRequest, IResult>, ISignUpAdminService
{
    private readonly UserManager<User> _userManager;

    public SignUpAdminService(ILogger<SignUpAdminService> logger,
        UserManager<User> userManager) : base(logger)
    {
        _userManager = userManager;
    }

    public async Task<IResult> HandleAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var exists = await _userManager.FindByEmailAsync(request.Email);
        if (exists.xIsNotEmpty()) return Results.Conflict();

        var user = new User()
        {
            Email = request.Email,
            UserName = request.Email,
            EmailConfirmed = true,
            PhoneNumber = null,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
            Type = ENUM_USER_TYPE.ADMIN,
            CreatedOn = DateTimeOffset.UtcNow
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);
        return TypedResults.Ok(user.Id.ToString());
    }
}