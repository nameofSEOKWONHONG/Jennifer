using FluentValidation;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;
using Jennifer.Jwt.Services.Abstracts;
using Jennifer.SharedKernel.Domains;
using Jennifer.SharedKernel.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Services;

public class UserService: IUserService
{
    private readonly ISessionContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<UserDto> _validator;

    public UserService(ISessionContext context, UserManager<User> userManager, IValidator<UserDto> validator)
    {
        _context = context;
        _userManager = userManager;
        _validator = validator;
    }

    public async Task<ApiResponse<IList<UserDto>>> GetUsers(string email, int page, int size, CancellationToken ct)
    {
        var query = _context.DbContext.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(x => x.Email.Contains(email));       
        }
        var result = await query
            .Skip((page - 1) * size)
            .Take(size)
            .Select(m => new UserDto()
            {
                Id = m.Id,
                Email = m.Email,
                UserName = m.UserName,
                PhoneNumber = m.PhoneNumber
            })
            .ToArrayAsync(cancellationToken: ct);
        
        return await ApiResponse<IList<UserDto>>.SuccessAsync(result);
    }

    public async Task<ApiResponse<UserDto>> GetUser(Guid id, CancellationToken ct)
    {
        var result = await _context.DbContext.Users.AsNoTracking()
            .Where(m => m.Id == id)
            .Select(m => new UserDto()
            {
                Id = m.Id,
                Email = m.Email,
                UserName = m.UserName,
                PhoneNumber = m.PhoneNumber
            })
            .FirstAsync(m => m.Id == id, ct);
        
        return await ApiResponse<UserDto>.SuccessAsync(result);
    }
    
    public async Task<ApiResponse<string>> AddUser(UserDto userAddRequest)
    {
        var validationResult = await _validator.ValidateAsync(userAddRequest);
        if (!validationResult.IsValid)
        {
            return await ApiResponse<string>.FailAsync("failed", validationResult.Errors.Select(m => new {m.PropertyName, m.ErrorMessage}));
        }
        var user = new User
        {
            UserName = userAddRequest.UserName,
            NormalizedUserName = userAddRequest.UserName.ToUpper(),
            Email = userAddRequest.Email,
            NormalizedEmail = userAddRequest.Email.ToUpper(),
            PhoneNumber = userAddRequest.PhoneNumber,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
        };
        var result = await _userManager.CreateAsync(user, userAddRequest.Password);
        if (!result.Succeeded)
        {
            // 충돌 에러 존재하면 409, 그 외는 400
            if (result.Errors.Any(e => e.Code == "DuplicateUserName" || e.Code == "DuplicateEmail"))
            {
                return await ApiResponse<string>.FailAsync("already exists", result.Errors.Select(m => new {m.Code, m.Description}));
            }

            return await ApiResponse<string>.FailAsync("failed", result.Errors.Select(m => new {m.Code, m.Description}));
        }

        return await ApiResponse<string>.SuccessAsync(user.Id.ToString());
    }

    public async Task<ApiResponse<bool>> ModifyUser(UserDto userDto, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(userDto.Id.ToString());
        if(user is null) return ApiResponse<bool>.Fail("not found");
        
        user.UserName = userDto.UserName;
        user.NormalizedUserName = userDto.UserName.ToUpper();
        user.Email = userDto.Email;
        user.NormalizedEmail = userDto.Email.ToUpper();
        user.PhoneNumber = userDto.PhoneNumber;
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return await ApiResponse<bool>.FailAsync("failed", result.Errors.Select(m => new {m.Code, m.Description}));
        }
        
        return await ApiResponse<bool>.SuccessAsync(true);
    }

    public async Task<ApiResponse<bool>> RemoveUser(Guid id, CancellationToken ct)
    {
        var user = await _context.DbContext.Users.FirstAsync(m => m.Id == id, ct);
        if(user is null) return ApiResponse<bool>.Fail("not found");

        user.UserName = string.Empty;
        user.NormalizedUserName = string.Empty;
        user.Email = string.Empty;
        user.NormalizedEmail = string.Empty;
        user.PhoneNumber = string.Empty;
        user.PasswordHash = string.Empty;
        user.LockoutEnabled = true;
        user.IsDelete = true;
        _context.DbContext.Users.Update(user);
        await _context.DbContext.SaveChangesAsync(ct);
        
        return await ApiResponse<bool>.SuccessAsync(true);
    }
}