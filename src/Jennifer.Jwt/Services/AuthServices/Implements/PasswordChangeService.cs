using System.Security.Claims;
using eXtensionSharp;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.Jwt.Services.Bases;
using Jennifer.SharedKernel.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;


public class PasswordChangeService: ServiceBase<PasswordChangeService,PasswordChangeRequest, ApiResponse<bool>>, IPasswordChangeService 
{
    private readonly UserManager<User> _userManager;
    private readonly HttpContext _httpContext;

    public PasswordChangeService(ILogger<PasswordChangeService> logger,
        UserManager<User> userManager, 
        IHttpContextAccessor accessor) : base(logger)
    {
        _userManager = userManager;
        _httpContext = accessor.HttpContext;
    }

    public async Task<ApiResponse<bool>> HandleAsync(PasswordChangeRequest request, CancellationToken cancellationToken)
    {
        var userId = _httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var user = await _userManager.FindByIdAsync(userId);
        if(user.xIsEmpty()) return await ApiResponse<bool>.FailAsync("Not found");
        
        var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (!result.Succeeded) return await ApiResponse<bool>.FailAsync(result.Errors.Select(m => m.Description).First());
        
        return await ApiResponse<bool>.SuccessAsync(result.Succeeded);
    }
}