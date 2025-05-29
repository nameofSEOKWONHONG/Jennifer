using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.Domain.Account;
using Jennifer.Infrastructure.Abstractions.ServiceCore;
using Jennifer.SharedKernel;

namespace Jennifer.Account.Application.Auth.Services.Abstracts;

internal interface IGenerateTokenService: IServiceBase<User, Result<TokenResponse>>;