using Jennifer.Account.Application.Auth.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Account.Application.Auth.Services.Abstracts;

internal interface IExternalOAuthService: IServiceBase<ExternalSignInRequest, TokenResponse>
{
}