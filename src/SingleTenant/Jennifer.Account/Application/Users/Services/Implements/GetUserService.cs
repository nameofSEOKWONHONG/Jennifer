// using Jennifer.Account.Application.Auth.Contracts;
// using Jennifer.Account.Application.Users.Services.Abstracts;
// using Jennifer.Account.Data;
// using Jennifer.Account.Services.UserServices.Abstracts;
// using Jennifer.Account.Session;
// using Jennifer.Account.Session.Abstracts;
// using Jennifer.SharedKernel;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
//
// namespace Jennifer.Account.Application.Users.Services.Implements;
//
// public class GetUserService : SessionServiceBase<GetUserService, Guid, Result<UserDto>>, IGetUserService
// {
//     public GetUserService(ILogger<GetUserService> logger, ISessionContext sessionContext) : base(logger, sessionContext)
//     {
//     }
//
//     public async Task<Result<UserDto>> HandleAsync(Guid request, CancellationToken cancellationToken)
//     {
//         var dbContext = this.AsDatabase<JenniferDbContext>();
//         var result = await dbContext.Users.AsNoTracking()
//             .Where(m => m.Id == request)
//             .Select(m => new UserDto()
//             {
//                 Id = m.Id,
//                 Email = m.Email,
//                 UserName = m.UserName,
//                 PhoneNumber = m.PhoneNumber
//             })
//             .FirstAsync(cancellationToken);
//         
//         return Result<UserDto>.Success(result);
//     }
// }