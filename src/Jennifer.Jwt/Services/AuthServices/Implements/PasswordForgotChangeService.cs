using eXtensionSharp;
using Jennifer.Jwt.Abstractions;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class PasswordForgotChangeService: ServiceBase<PasswordForgotChangeService, PasswordForgotChangeRequest, IResult>, IPasswordForgotChangeService
{
    private readonly JenniferDbContext _applicationDbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IVerifyCodeService _service;

    public PasswordForgotChangeService(ILogger<PasswordForgotChangeService> logger,
        JenniferDbContext applicationDbContext,
        IPasswordHasher<User> passwordHasher,
        IVerifyCodeService service) : base(logger)
    {
        _applicationDbContext = applicationDbContext;
        _passwordHasher = passwordHasher;
        _service = service;
    }

    public async Task<IResult> HandleAsync(PasswordForgotChangeRequest request, CancellationToken cancellationToken)
    {
        var verfied = await _service.HandleAsync(new VerifyCodeRequest(request.Email, request.Code, request.Type), cancellationToken);
        if(verfied.Status != ENUM_VERITY_RESULT_STATUS.EMAIL_CONFIRM)
            return Results.BadRequest(verfied);
        
        var user = await _applicationDbContext.Users.FirstOrDefaultAsync(m => m.Email == request.Email, cancellationToken: cancellationToken);
        if(user.xIsEmpty()) return Results.NotFound();
        
        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        user.ConcurrencyStamp = Guid.NewGuid().ToString();
        user.SecurityStamp = Guid.NewGuid().ToString();
        
        await _applicationDbContext.SaveChangesAsync(cancellationToken); // ✅ 저장 누락 방지
        
        return Results.Ok();
    }
}