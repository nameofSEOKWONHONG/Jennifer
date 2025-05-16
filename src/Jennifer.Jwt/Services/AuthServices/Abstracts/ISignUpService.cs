using Jennifer.Jwt.Services.AuthServices.Contracts;
using Jennifer.Jwt.Services.Bases;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface ISignUpService: IServiceBase<RegisterRequest, IResult>
{
    
}