using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;

namespace Jennifer.Web.Admin.Client.Auth.Services
{
    public interface ILoginService
    {
        Task<Result<TokenResponse>> LoginAsync(string username, string password);
    }
}
