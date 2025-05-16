using Jennifer.Jwt.Services.Bases;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface IRefreshTokenService: IServiceBase<string, IResult>
{
    
}