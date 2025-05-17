using Jennifer.Jwt.Data;
using Jennifer.Jwt.Domains;
using Jennifer.Jwt.Services.UserServices.Abstracts;
using Jennifer.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Jennifer.Jwt.Services.UserServices.Implements;

public class GetUserService : SessionServiceBase<GetUserService, Guid, ApiResponse<UserDto>>, IGetUserService
{
    public GetUserService(ILogger<GetUserService> logger, ISessionContext sessionContext) : base(logger, sessionContext)
    {
    }

    public async Task<ApiResponse<UserDto>> HandleAsync(Guid request, CancellationToken cancellationToken)
    {
        var dbContext = this.AsDatabase<JenniferDbContext>();
        var result = await dbContext.Users.AsNoTracking()
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