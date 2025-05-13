using Jennifer.Jwt.Data;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Models;
using Jennifer.SharedKernel.Domains;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jennifer.Jwt.Services;

public class UserService: IUserService
{
    private readonly ISessionContext _context;
    private readonly UserManager<User> _userManager;

    public UserService(ISessionContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<User>> GetUsers(string email, int page, int size, CancellationToken ct)
    {
        var query = _context.DbContext.Users.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(email))
        {
            query = query.Where(x => x.Email.Contains(email));       
        }
        return await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToArrayAsync(cancellationToken: ct);
    }

    public async Task<User> GetUser(string id, CancellationToken ct)
    {
        if (!Guid.TryParse(id, out var guid)) throw new ArgumentException("Parameter is not a valid Guid.");
        return await _context.DbContext.Users.AsNoTracking()
            .Where(m => m.Id == guid)
            .FirstAsync(m => m.Id == guid, ct);
    }
    
    public async Task<ApiResponse<string>> AddUser(UserDto userDto)
    {
        var user = new User
        {
            UserName = userDto.Name,
            NormalizedUserName = userDto.Name.ToUpper(),
            Email = userDto.Email,
            NormalizedEmail = userDto.Email.ToUpper(),
            PhoneNumber = userDto.PhoneNumber,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0,
        };
        var result = await _userManager.CreateAsync(user, userDto.Password);
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
        
        user.UserName = userDto.Name;
        user.NormalizedUserName = userDto.Name.ToUpper();
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
        user.IsDelete = true;
        _context.DbContext.Users.Update(user);
        await _context.DbContext.SaveChangesAsync(ct);
        
        return ApiResponse<bool>.Success(true);
    }
}