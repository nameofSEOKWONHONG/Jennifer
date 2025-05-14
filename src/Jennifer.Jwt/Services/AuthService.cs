using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.SharedKernel.Domains;
using Jennifer.SharedKernel.Infrastructure.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;

namespace Jennifer.Jwt.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IJwtService _jwtService;
    private readonly HttpContext _httpContext;
    private readonly IEmailQueue _emailQueue;
    
    public AuthService(UserManager<User> userManager, 
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IJwtService jwtService,
        IEmailQueue emailQueue,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _jwtService = jwtService;
        _emailQueue = emailQueue;
        _httpContext = httpContextAccessor.HttpContext;
    }
    
    public async Task<IResult> Register(RegisterRequest request)
    {
        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            EmailConfirmed = true,
            PhoneNumber = null,
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

        return Results.Ok();
    }

    public async Task<TokenResponse> Signin(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user is null) return null;
        
        if(!await _userManager.CheckPasswordAsync(user, password)) return null;
        
        // var result = await _signInManager.PasswordSignInAsync
        //     (user, password, isPersistent:false, lockoutOnFailure:false);
        // if (result.Succeeded)
        // {
        //
        // }
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    roleClaims.Add(claim); // ClaimType/Value 그대로
                }
            }
        }

        var refreshToken = _jwtService.GenerateRefreshToken();
        var refreshTokenObj = new RefreshToken(refreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:refreshToken);
        var encodedRefreshToken = _jwtService.GenerateRefreshTokenString(refreshTokenObj);
        return new TokenResponse(_jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), encodedRefreshToken);
    }

    public async Task<bool> CookieSignIn(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if(user is null) return false;
        
        var result = await _signInManager.PasswordSignInAsync
            (user, password, isPersistent:false, lockoutOnFailure:false);
        return result.Succeeded;
    }

    public async Task<bool> SignOut()
    {
        var id = _httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if(string.IsNullOrWhiteSpace(id)) return false;
        var user = await _userManager.FindByIdAsync(id);
        if(user is null) return false;
        await _userManager.RemoveAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        return true;
    }

    public async Task<TokenResponse> RefreshToken(string refreshToken)
    {
        var refreshTokenObj = _jwtService.GenerateRefreshToken(refreshToken);
        if(refreshTokenObj is null) return null;
        
        if(refreshTokenObj.Expiry < DateTime.UtcNow) return null;
        
        var user = await _userManager.FindByIdAsync(refreshTokenObj.UserId);
        if(user is null) return null;
        
        if(refreshTokenObj.UserId != user.Id.ToString()) return null;
        
        var token = await _userManager.GetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken");
        if(token is null) return null;
        
        if(refreshTokenObj.Token != token) return null;
        
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        foreach (var roleName in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role != null)
            {
                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    roleClaims.Add(claim); // ClaimType/Value 그대로
                }
            }
        }
        
        var newRefreshToken = _jwtService.GenerateRefreshToken();
        var newRefreshTokenObj = new RefreshToken(newRefreshToken, DateTime.UtcNow.AddDays(7), DateTime.UtcNow, user.Id.ToString());
        await _userManager.SetAuthenticationTokenAsync(user, loginProvider:"internal", tokenName:"refreshToken", tokenValue:newRefreshToken);
        return new TokenResponse(_jwtService.GenerateJwtToken(user, userClaims.ToList(), roleClaims), _jwtService.GenerateRefreshTokenString(newRefreshTokenObj));
    }
    
    public async Task<string> RequestChangePasswordToken(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return null;

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        return token;
    }

    public async Task<bool> ChangePasswordWithToken(string token, string oldPassword, string newPassword)
    {
        var id = _httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return false;
        
        var isMatch = await _userManager.CheckPasswordAsync(user, oldPassword);
        if (!isMatch) return false;

        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> RequestConfirmEmailToken(string url, string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;
        
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        
        //this code is for test, not for production. not working.
        var mail = new EmailMessage.Builder()
            .From("test", "test@test.com")
            .To(string.Empty, email)
            .Subject("Jennifer Email Confirmation")
            .Body($"<p>https://example.com/confirmemail?userId={user.Id.ToString()}token={token}</p>", true)
            .Build();

        await _emailQueue.EnqueueAsync(mail);
        
        return true;
    }
    
    public async Task<bool> ConfirmEmail(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        
        var result = await _userManager.ConfirmEmailAsync(user, code);
        return result.Succeeded;
    }
    
    public async Task<bool> UpdateUserInfo(string email, string name, string phoneNumber)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;
        
        user.UserName = name;
        user.PhoneNumber = phoneNumber;
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }
}

