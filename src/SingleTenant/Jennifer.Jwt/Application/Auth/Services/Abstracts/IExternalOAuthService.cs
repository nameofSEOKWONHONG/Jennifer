using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Application.Auth.Services.Abstracts;

public interface IExternalOAuthService: IServiceBase<ExternalSignInRequest, IResult>
{
}