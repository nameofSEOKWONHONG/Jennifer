using eXtensionSharp;
using Jennifer.Jwt.Abstractions;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.AuthServices.Abstracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.AuthServices.Implements;

public class CheckEmailService: ServiceBase<CheckEmailService, string, IResult>, ICheckEmailService
{
    private readonly UserManager<User> _userManager;

    public CheckEmailService(ILogger<CheckEmailService> logger,
        UserManager<User> userManager) : base(logger)
    {
        _userManager = userManager;
    }

    public async Task<IResult> HandleAsync(string request, CancellationToken cancellationToken)
    {
        var result = await _userManager.FindByEmailAsync(request);
        if (result.xIsEmpty()) return Results.NotFound();
        return Results.Ok();
    }
}