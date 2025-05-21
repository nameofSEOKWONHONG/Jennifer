using Jennifer.Infrastructure.Abstractions;
using Jennifer.Jwt.Application.Auth.Contracts;
using Jennifer.Jwt.Data;
using Jennifer.Jwt.Services.UserServices.Abstracts;
using Jennifer.Jwt.Session;
using Jennifer.Jwt.Session.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.UserServices.Implements;

public class GetUserService : SessionServiceBase<GetUserService, Guid, ApiResponse<UserDto>>, IGetUserService
{
    private readonly JenniferDbContext _dbContext;

    public GetUserService(ILogger<GetUserService> logger, ISessionContext sessionContext,
        JenniferDbContext dbContext) : base(logger, sessionContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<UserDto>> HandleAsync(Guid request, CancellationToken cancellationToken)
    {
        var result = await _dbContext.Users.AsNoTracking()
            .Where(m => m.Id == request)
            .Select(m => new UserDto()
            {
                Id = m.Id,
                Email = m.Email,
                UserName = m.UserName,
                PhoneNumber = m.PhoneNumber
            })
            .FirstAsync(cancellationToken);
        
        return await ApiResponse<UserDto>.SuccessAsync(result);
    }
}