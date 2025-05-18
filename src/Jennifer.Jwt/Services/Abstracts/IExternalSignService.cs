using Jennifer.Jwt.Application.Auth.Services.Contracts;
using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Services.Abstracts;

public interface IExternalSignService: IServiceBase<ExternalSignInRequest, IResult>
{
}