using Jennifer.SharedKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public interface ISignUpAdminService : IServiceBase<RegisterRequest, IResult>
{
    
}