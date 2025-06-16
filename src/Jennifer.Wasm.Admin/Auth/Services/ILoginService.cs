using Jennifer.SharedKernel;
using Jennifer.SharedKernel.Account.Auth;

namespace Jennifer.Wasm.Admin.Auth.Services
{
    public interface ILoginService
    {
        Task<Result<TokenResponse>> LoginAsync(string username, string password);
    }
}
