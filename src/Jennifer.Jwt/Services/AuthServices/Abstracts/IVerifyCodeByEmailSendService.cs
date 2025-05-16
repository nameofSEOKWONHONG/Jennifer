using Jennifer.Jwt.Services.Bases;
using Microsoft.AspNetCore.Http;

namespace Jennifer.Jwt.Services.AuthServices.Abstracts;

public record VerifyCodeByEmailSendRequest(string Email, string Type);

public interface IVerifyCodeByEmailSendService : IServiceBase<VerifyCodeByEmailSendRequest, IResult>
{
    
}