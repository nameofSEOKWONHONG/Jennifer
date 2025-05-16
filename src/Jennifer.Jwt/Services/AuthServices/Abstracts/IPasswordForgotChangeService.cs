using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.SharedKernel.Base;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface IPasswordForgotChangeService: IServiceBase<PasswordForgotChangeRequest, IResult>
{
    
}