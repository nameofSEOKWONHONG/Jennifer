using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Base;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Services.Abstracts;

public interface IExternalSignService: IServiceBase<ExternalSignInRequest, IResult>
{
}